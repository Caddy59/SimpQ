using SimpQ.Abstractions.Models.Internal;
using SimpQ.Abstractions.Models.Requests;
using SimpQ.Abstractions.Reports;

namespace SimpQ.Abstractions.Queries;

/// <summary>
/// Defines a factory responsible for constructing SQL query definitions,
/// including row and count queries, based on query parameters.
/// </summary>
public interface IQueryDefinitionFactory {
    /// <summary>
    /// Builds a non-paged query definition using the specified base and pre-query SQL, 
    /// along with generic query parameters.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to map the result to. Must implement <see cref="IReportEntity"/>.</typeparam>
    /// <param name="rawQuery">The main SQL query used to retrieve data (typically a SELECT statement).</param>
    /// <param name="rawInitSql">Optional SQL that runs before the base query (e.g., CTEs or temp tables).</param>
    /// <param name="queryParams">The query parameters including select, filters, and ordering.</param>
    /// <returns>A <see cref="QueryDefinition"/> representing the row and count commands with parameters.</returns>
    QueryDefinition BuildQueryDefinition<TEntity>(string rawQuery, string rawInitSql, QueryParams queryParams) where TEntity : IReportEntity, new();

    /// <summary>
    /// Builds a query definition configured for offset-based pagination.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to map the result to. Must implement <see cref="IReportEntity"/>.</typeparam>
    /// <param name="rawQuery">The main SQL query used to retrieve data (typically a SELECT statement).</param>
    /// <param name="rawInitSql">Optional SQL that runs before the base query (e.g., CTEs or temp tables).</param>
    /// <param name="queryParams">The query parameters including select, filters, ordering and page size.</param>
    /// <param name="limit">Maximum number of rows allowed in the result set.</param>
    /// <returns>A <see cref="QueryDefinition"/> representing the row and count commands with parameters.</returns>
    QueryDefinition BuildOffsetPagedQueryDefinition<TEntity>(string rawQuery, string rawInitSql, OffsetPagedQueryParams queryParams, int limit) where TEntity : IReportEntity, new();

    /// <summary>
    /// Builds a query definition configured for keyset-based pagination.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to map the result to. Must implement <see cref="IReportEntity"/>.</typeparam>
    /// <param name="rawQuery">The main SQL query used to retrieve data (typically a SELECT statement).</param>
    /// <param name="rawInitSql">Optional SQL to run before the main query (e.g., setting up temp tables).</param>
    /// <param name="queryParams">Keyset pagination parameters including cursor and page size.</param>
    /// <returns>A <see cref="QueryDefinition"/> representing the row and count commands with parameters.</returns>
    QueryDefinition BuildKeysetPagedQueryDefinition<TEntity>(string rawQuery, string rawInitSql, KeysetPagedQueryParams queryParams) where TEntity : IReportEntity, new();
}