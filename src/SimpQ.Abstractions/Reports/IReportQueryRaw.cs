using SimpQ.Abstractions.Models.Requests;
using SimpQ.Abstractions.Models.Results;

namespace SimpQ.Abstractions.Reports;

/// <summary>
/// Defines a set of methods for executing raw SQL queries and returning typed results, 
/// supporting distinct types of pagination.
/// </summary>
public interface IReportQueryRaw {
    /// <summary>
    /// Executes a raw SQL query and returns the result as a non-paged collection.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to map the result to. Must implement <see cref="IReportEntity"/>.</typeparam>
    /// <param name="rawQuery">The main SQL query to execute (SELECT statement).</param>
    /// <param name="queryParams">The parameters to apply.</param>
    /// <param name="timeout">Command timeout in milliseconds (optional). Default is 30 seconds.</param>
    /// <param name="cancellationToken">Token to cancel the operation (optional).</param>
    /// <returns>A task that returns a <see cref="QueryResult{TEntity}"/> containing the query data.</returns>
    Task<QueryResult<TEntity>> ExecuteQueryAsync<TEntity>(string rawQuery, QueryParams queryParams, int timeout = 30000, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new();

    /// <summary>
    /// Executes a raw SQL query and returns the result as a non-paged collection.
    /// Useful for setting up temp tables or CTEs before querying.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to map the result to. Must implement <see cref="IReportEntity"/>.</typeparam>
    /// <param name="rawQuery">The main SQL query to execute (SELECT statement).</param>
    /// <param name="rawInitSql">An SQL statement to execute before the main query.</param>
    /// <param name="queryParams">The parameters to apply.</param>
    /// <param name="timeout">Command timeout in milliseconds (optional). Default is 30 seconds.</param>
    /// <param name="cancellationToken">Token to cancel the operation (optional).</param>
    /// <returns>A task that returns a <see cref="QueryResult{TEntity}"/> containing the query data.</returns>
    Task<QueryResult<TEntity>> ExecuteQueryAsync<TEntity>(string rawQuery, string rawInitSql, QueryParams queryParams, int timeout = 30000, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new();

    /// <summary>
    /// Executes a raw SQL query with offset-based pagination and returns the paginated result.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to map the result to. Must implement <see cref="IReportEntity"/>.</typeparam>
    /// <param name="rawQuery">The main SQL query to execute (SELECT statement).</param>
    /// <param name="queryParams">The offset pagination parameters to apply.</param>
    /// <param name="limit">Maximum number of rows allowed for pagination (optional). Default is 10.000.</param>
    /// <param name="timeout">Command timeout in milliseconds (optional). Default is 30 seconds.</param>
    /// <param name="cancellationToken">Token to cancel the operation (optional).</param>
    /// <returns>A task that returns an <see cref="OffsetPagedQueryResult{TEntity}"/> containing paged results and metadata.</returns>
    Task<OffsetPagedQueryResult<TEntity>> ExecuteOffsetPagedQueryAsync<TEntity>(string rawQuery, OffsetPagedQueryParams queryParams, int limit = 10000, int timeout = 30000, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new();

    /// <summary>
    /// Executes a raw SQL query with offset-based pagination and returns the paginated result.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to map the result to. Must implement <see cref="IReportEntity"/>.</typeparam>
    /// <param name="rawQuery">The main SQL query to execute (SELECT statement).</param>
    /// <param name="rawInitSql">An SQL statement to execute before the main query.</param>
    /// <param name="queryParams">The offset pagination parameters to apply.</param>
    /// <param name="limit">Maximum number of rows allowed for pagination (optional). Default is 10.000.</param>
    /// <param name="timeout">Command timeout in milliseconds (optional). Default is 30 seconds.</param>
    /// <param name="cancellationToken">Token to cancel the operation (optional).</param>
    /// <returns>A task that returns an <see cref="OffsetPagedQueryResult{TEntity}"/> containing paged results and metadata.</returns>
    Task<OffsetPagedQueryResult<TEntity>> ExecuteOffsetPagedQueryAsync<TEntity>(string rawQuery, string rawInitSql, OffsetPagedQueryParams queryParams, int limit = 10000, int timeout = 30000, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new();

    /// <summary>
    /// Executes a raw SQL query with keyset-based pagination and returns the paginated result.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to map the result to. Must implement <see cref="IReportEntity"/>.</typeparam>
    /// <param name="rawQuery">The main SQL query to execute (SELECT statement).</param>
    /// <param name="queryParams">The keyset pagination parameters to apply.</param>
    /// <param name="timeout">Command timeout in milliseconds (optional). Default is 30 seconds.</param>
    /// <param name="cancellationToken">Token to cancel the operation (optional).</param>
    /// <returns>A task that returns a <see cref="KeysetPagedQueryResult{TEntity}"/> containing the paged data and keyset metadata.</returns>
    Task<KeysetPagedQueryResult<TEntity>> ExecuteKeysetPagedQueryAsync<TEntity>(string rawQuery, KeysetPagedQueryParams queryParams, int timeout = 30000, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new();

    /// <summary>
    /// Executes a raw SQL query with keyset-based pagination and returns the paginated result.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to map the result to. Must implement <see cref="IReportEntity"/>.</typeparam>
    /// <param name="rawQuery">The main SQL query to execute (SELECT statement).</param>
    /// <param name="rawInitSql">An SQL statement to execute before the main query.</param>
    /// <param name="queryParams">The keyset pagination parameters to apply.</param>
    /// <param name="timeout">Command timeout in milliseconds (optional). Default is 30 seconds.</param>
    /// <param name="cancellationToken">Token to cancel the operation (optional).</param>
    /// <returns>A task that returns a <see cref="KeysetPagedQueryResult{TEntity}"/> containing the paged data and keyset metadata.</returns>
    Task<KeysetPagedQueryResult<TEntity>> ExecuteKeysetPagedQueryAsync<TEntity>(string rawQuery, string rawInitSql, KeysetPagedQueryParams queryParams, int timeout = 30000, CancellationToken cancellationToken = default) where TEntity : IReportEntity, new();
}