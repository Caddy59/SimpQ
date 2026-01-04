using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using SimpQ.Core.Configuration;
using SimpQ.Core.Helpers;

namespace SimpQ.SqlServer.Reports;

/// <summary>
/// Provides raw query execution against a SQL Server database using SimpQ's query definition framework.
/// Supports plain, offset-based, and keyset-based paginated queries.
/// </summary>
/// <param name="logger">The logger used for tracing generated SQL queries.</param>
/// <param name="connectionString">The SQL Server connection string used to establish the database connection.</param>
/// <param name="queryBuilder">The query definition factory responsible for building SQL commands and parameters.</param>
/// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
public class SqlServerReportQueryRaw(ILogger<SqlServerReportQueryRaw> logger, string connectionString, IQueryDefinitionFactory queryBuilder, EntityConfigurationRegistry? configurationRegistry = null) : IReportQueryRaw {
    /// <inheritdoc/>
    public async Task<QueryResult<TEntity>> ExecuteQueryAsync<TEntity>(string rawQuery, QueryParams queryParams, int timeout = 30000, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new() =>
        await ExecuteQueryAsync<TEntity>(rawQuery, string.Empty, queryParams, timeout, cancellationToken);

    /// <inheritdoc/>
    public async Task<QueryResult<TEntity>> ExecuteQueryAsync<TEntity>(string rawQuery, string rawInitSql, QueryParams queryParams, int timeout = 30000, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new() {
        var query = queryBuilder.BuildQueryDefinition<TEntity>(rawQuery, rawInitSql, queryParams);
        logger.LogTrace("Query generated:\n{Query}", query.RowsCommand);
        if (query.Parameters.Count > 0)
            logger.LogTrace("Parameters:\n{Parameters}", ParameterLoggingHelper.ToJsonWithSqlDbType(query.Parameters));
        var entities = await GetEntitiesAsync<TEntity>(query.RowsCommand, query.Parameters, timeout, cancellationToken);

        return new() {
            Data = entities
        };
    }

    /// <inheritdoc/>
    public async Task<OffsetPagedQueryResult<TEntity>> ExecuteOffsetPagedQueryAsync<TEntity>(string rawQuery, OffsetPagedQueryParams queryParams, int limit = 10000, int timeout = 30000, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new() =>
        await ExecuteOffsetPagedQueryAsync<TEntity>(rawQuery, string.Empty, queryParams, limit, timeout, cancellationToken);

    /// <inheritdoc/>
    public async Task<OffsetPagedQueryResult<TEntity>> ExecuteOffsetPagedQueryAsync<TEntity>(string rawQuery, string rawInitSql, OffsetPagedQueryParams queryParams, int limit = 10000, int timeout = 30000, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new() {
        var query = queryBuilder.BuildOffsetPagedQueryDefinition<TEntity>(rawQuery, rawInitSql, queryParams, limit);
        logger.LogTrace("Query generated:\n{Query}", query.RowsCommand);
        logger.LogTrace("Count query generated:\n{Query}", query.CountCommand);
        if (query.Parameters.Count > 0)
            logger.LogTrace("Parameters:\n{Parameters}", ParameterLoggingHelper.ToJsonWithSqlDbType(query.Parameters));
        var count = await GetCountAsync(query.CountCommand, query.Parameters, timeout, cancellationToken);
        var entities = await GetEntitiesAsync<TEntity>(query.RowsCommand, query.Parameters, timeout, cancellationToken);

        var pageSize = queryParams.PageSize;
        var pageCount = (int)Math.Ceiling((double)count / pageSize);

        return new() {
            Data = entities,
            CurrentPage = queryParams.Page,
            PageSize = pageSize,
            PageCount = pageCount,
            RowCount = count
        };
    }

    /// <inheritdoc/>
    public async Task<KeysetPagedQueryResult<TEntity>> ExecuteKeysetPagedQueryAsync<TEntity>(string rawQuery, KeysetPagedQueryParams queryParams, int timeout = 30000, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new() =>
        await ExecuteKeysetPagedQueryAsync<TEntity>(rawQuery, string.Empty, queryParams, timeout, cancellationToken);

    /// <inheritdoc/>
    public async Task<KeysetPagedQueryResult<TEntity>> ExecuteKeysetPagedQueryAsync<TEntity>(string rawQuery, string rawInitSql, KeysetPagedQueryParams queryParams, int timeout = 30000, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new() {
        var query = queryBuilder.BuildKeysetPagedQueryDefinition<TEntity>(rawQuery, rawInitSql, queryParams);
        logger.LogTrace("Query generated:\n{Query}", query.RowsCommand);
        if (query.Parameters.Count > 0)
            logger.LogTrace("Parameters:\n{Parameters}", ParameterLoggingHelper.ToJsonWithSqlDbType(query.Parameters));
        var entities = await GetEntitiesAsync<TEntity>(query.RowsCommand, query.Parameters, timeout, cancellationToken);

        var pageSize = queryParams.PageSize;
        var hasMore = entities.Count > pageSize;
        if (hasMore)
            entities = [.. entities.Take(entities.Count - 1)];

        var next = GetNext(entities, configurationRegistry);

        return new() {
            Data = entities,
            PageSize = pageSize,
            HasMore = hasMore,
            Next = next,
            IsDescending = queryParams.IsDescending
        };
    }

    /// <summary>
    /// Executes a SQL query and maps the resulting rows to a collection of <typeparamref name="TEntity"/> instances.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity to map each row to. Must implement <see cref="IReportEntity"/> and have a parameterless constructor.
    /// </typeparam>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">The parameters to bind to the SQL command.</param>
    /// <param name="timeout">The command timeout in milliseconds.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a read-only collection
    /// of <typeparamref name="TEntity"/> instances populated from the query result.
    /// </returns>
    private async Task<IReadOnlyCollection<TEntity>> GetEntitiesAsync<TEntity>(string query, IReadOnlyCollection<Parameter> parameters, int timeout, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new() {
        var entities = new List<TEntity>();
        using(var connection = new SqlConnection(connectionString)) {
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand(query, connection);
            var sqlParameters = EntityHelper.GetSqlParameters(parameters);
            command.Parameters.AddRange(sqlParameters);
            command.CommandTimeout = timeout;

            using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
            if(dataReader is not null && dataReader.HasRows) {
                while(await dataReader.ReadAsync(cancellationToken)) {
                    var entity = dataReader.GetEntity<TEntity>(configurationRegistry);
                    entities.Add(entity);
                }
            }
        }

        return entities;
    }

    /// <summary>
    /// Executes a parameterized scalar SQL query intended to return a row count (e.g., <c>SELECT COUNT(1)</c>).
    /// </summary>
    /// <param name="query">The SQL count query to execute.</param>
    /// <param name="parameters">The parameters to bind to the SQL command.</param>
    /// <param name="timeout">The command timeout in milliseconds.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the total number of rows matching the query conditions.
    /// </returns>
    private async Task<int> GetCountAsync(string query, IReadOnlyCollection<Parameter> parameters, int timeout, CancellationToken cancellationToken = default) {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        using var command = new SqlCommand(query, connection);
        var sqlParameters = EntityHelper.GetSqlParameters(parameters);
        command.Parameters.AddRange(sqlParameters);
        command.CommandTimeout = timeout;

        var scalar = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(scalar);
    }

    /// <summary>
    /// Extracts keyset pagination values from the last item in a query result set.
    /// These values are used as the cursor for retrieving the next page of results.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity returned by the query. Must implement <see cref="IReportEntity"/> and have a parameterless constructor.
    /// </typeparam>
    /// <param name="entities">The collection of entities returned from the current query execution.</param>
    /// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
    /// <returns>
    /// A read-only dictionary containing the property names and values used as the keyset cursor,
    /// or <c>null</c> if the input collection is empty.
    /// </returns>
    private static ReadOnlyDictionary<string, object?>? GetNext<TEntity>(IReadOnlyCollection<TEntity> entities, EntityConfigurationRegistry? configurationRegistry = null) where TEntity : IReportEntity, new() {
        if (entities.Count == 0)
            return default;

        var keysetProperties = QueryHelperExtensions.GetOrderedKeysetProperties<TEntity>(configurationRegistry);
        var entity = entities.Last();
        var next = new Dictionary<string, object?>();

        foreach (var keysetProperty in keysetProperties) {
            var value = keysetProperty.GetValue(entity);
            next[keysetProperty.Name] = value;
        }

        return next.AsReadOnly();
    }
}