using SimpQ.SqlServer.Models;
using SimpQ.SqlServer.Queries.ClauseBuilders;

namespace SimpQ.SqlServer.Queries;

/// <summary>
/// SQL Server-specific implementation of <see cref="IQueryDefinitionFactory"/> that builds
/// parameterized SQL queries for SimpQ's filtering, ordering, and pagination features.
/// </summary>
/// <param name="whereClauseBuilder">Builds the WHERE clause based on filters and keyset pagination.</param>
/// <param name="orderClauseBuilder">Builds the ORDER BY clause based on user or default sort rules.</param>
public class SqlServerQueryDefinitionFactory(WhereClauseBuilder whereClauseBuilder, OrderClauseBuilder orderClauseBuilder) : IQueryDefinitionFactory {
    /// <inheritdoc/>
    public QueryDefinition BuildQueryDefinition<TEntity>(string rawQuery, string rawInitSql, QueryParams queryParams) where TEntity : IReportEntity, new() {
        var input = new QueryDefinitionInput(rawQuery, rawInitSql, queryParams.Select, queryParams.Filters, queryParams.Order);
        return BuildQueryDefinitionInternal<TEntity>(input);
    }
        

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="queryParams.PageSize"/> exceeds the specified <paramref name="limit"/>.</exception>
    public QueryDefinition BuildOffsetPagedQueryDefinition<TEntity>(string rawQuery, string rawInitSql, OffsetPagedQueryParams queryParams, int limit) where TEntity : IReportEntity, new() {
        if (queryParams.PageSize > limit)
            throw new InvalidOperationException($"Page size cannot exceed {limit}. Current page size: {queryParams.PageSize}.");
        
        var offset = (queryParams.Page - 1) * queryParams.PageSize;
        var paginationClause = PaginationClauseBuilder.Build(offset, queryParams.PageSize);

        var input = new QueryDefinitionInput(rawQuery, rawInitSql, queryParams.Select, queryParams.Filters, queryParams.Order, AppendCountQuery: true, PaginationClause: paginationClause);
        return BuildQueryDefinitionInternal<TEntity>(input);
    }

    /// <inheritdoc/>
    public QueryDefinition BuildKeysetPagedQueryDefinition<TEntity>(string rawQuery, string rawInitSql, KeysetPagedQueryParams queryParams) where TEntity : IReportEntity, new() {        
        var keysetFilter = new KeysetFilter(queryParams.Next, queryParams.IsDescending);
        var paginationClause = PaginationClauseBuilder.Build(0, queryParams.PageSize + 1);
        var input = new QueryDefinitionInput(rawQuery, rawInitSql, queryParams.Select, queryParams.Filters, queryParams.Order, KeysetFilter: keysetFilter, PaginationClause: paginationClause);

        return BuildQueryDefinitionInternal<TEntity>(input);
    }

    /// <summary>
    /// Core builder method that assembles all parts of a query—<c>SELECT</c>, <c>WHERE</c>, <c>ORDER BY</c>,
    /// and pagination clauses—based on the provided <paramref name="input"/> configuration.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity the query is targeting. Must implement <see cref="IReportEntity"/> and have a parameterless constructor.
    /// </typeparam>
    /// <param name="input">
    /// The complete set of inputs required to construct the query definition, including the base query,
    /// optional pre-query SQL, filters, ordering, selection fields, pagination logic, and keyset state.
    /// </param>
    /// <returns>
    /// A fully composed <see cref="QueryDefinition"/> representing the SQL query and its parameter collection.
    /// </returns>
    private QueryDefinition BuildQueryDefinitionInternal<TEntity>(QueryDefinitionInput input) where TEntity : IReportEntity, new() {
        var selectClause = SelectClauseBuilder.Build<TEntity>(input.Select, input.RawQuery);
        var whereClause = whereClauseBuilder.Build<TEntity>(input.Filters, input.KeysetFilter, out var parameters);
        var orderByClause = orderClauseBuilder.Build<TEntity>(input.Order, input.KeysetFilter);
        
        var fullQuery = string.Join(Environment.NewLine,
            selectClause,
            whereClause,
            orderByClause,
            input.PaginationClause);

        if (!string.IsNullOrWhiteSpace(input.RawInitSql))
            fullQuery = PrependInitSql(fullQuery, input.RawInitSql);

        var countQuery = string.Empty;
        if (input.AppendCountQuery)
            countQuery = GetCountQuery(input.RawInitSql, $"{selectClause}{Environment.NewLine}{whereClause}");

        return new QueryDefinition(fullQuery, countQuery, parameters);
    }

    /// <summary>
    /// Prepends initialization SQL (e.g., CTEs or temp table setup) to the main query.
    /// </summary>
    private static string PrependInitSql(string sql, string rawInitSql) =>
        string.IsNullOrWhiteSpace(rawInitSql) ? sql : $"{rawInitSql}{Environment.NewLine}{sql}";

    /// <summary>
    /// Builds a parameterized SQL count query that wraps the main query body.
    /// </summary>
    /// <param name="rawInitSql">Optional SQL to prepend before the count query.</param>
    /// <param name="body">The core SELECT + WHERE clause to count over.</param>
    /// <returns>A complete SQL statement that returns the total row count.</returns>
    private static string GetCountQuery(string rawInitSql, string body) {
        var baseQuery = $"SELECT{Environment.NewLine}COUNT(1){Environment.NewLine}FROM ({body}) \"count\";";
        return string.IsNullOrWhiteSpace(rawInitSql) ? baseQuery : $"{rawInitSql}{Environment.NewLine}{baseQuery}";
    }
}