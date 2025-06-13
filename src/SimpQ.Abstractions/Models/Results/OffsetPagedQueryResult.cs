using SimpQ.Abstractions.Reports;

namespace SimpQ.Abstractions.Models.Results;

/// <summary>
/// Represents the result of a query that uses offset-based pagination,
/// including metadata such as the current page and total row count.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity returned in the query result. Must implement <see cref="IReportEntity"/> and have a parameterless constructor.
/// </typeparam>
public sealed record OffsetPagedQueryResult<TEntity> : QueryResult<TEntity> where TEntity : IReportEntity, new() {
    /// <summary>
    /// Gets the current page number (1-based index).
    /// </summary>
    public int CurrentPage { get; init; }

    /// <summary>
    /// Gets the number of items returned per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Gets the total number of pages based on the row count and page size.
    /// </summary>
    public int PageCount { get; init; }

    /// <summary>
    /// Gets the total number of rows that match the query criteria (ignoring pagination).
    /// </summary>
    public int RowCount { get; init; }
}