using SimpQ.Abstractions.Reports;

namespace SimpQ.Abstractions.Models.Results;

/// <summary>
/// Represents the result of a query that uses keyset-based pagination,
/// including metadata needed to retrieve the next page of results.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity returned in the query result. Must implement <see cref="IReportEntity"/> and have a parameterless constructor.
/// </typeparam>
public sealed record KeysetPagedQueryResult<TEntity> : QueryResult<TEntity> where TEntity : IReportEntity, new() {
    /// <summary>
    /// Gets the number of items returned per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Gets a value indicating whether there are more results available after this page.
    /// </summary>
    public bool HasMore { get; init; }

    /// <summary>
    /// Gets the key values of the last row in the current result set.
    /// These values should be used as the starting point for fetching the next page.
    /// </summary>
    public IReadOnlyDictionary<string, object?>? Next { get; init; }

    /// <summary>
    /// Gets a value indicating whether the results were sorted in descending order.
    /// </summary>
    public bool IsDescending { get; init; }
}