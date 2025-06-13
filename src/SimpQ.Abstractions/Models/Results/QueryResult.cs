using SimpQ.Abstractions.Reports;

namespace SimpQ.Abstractions.Models.Results;

/// <summary>
/// Represents the result of a query operation, containing a collection of entities.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity returned in the query result. Must implement <see cref="IReportEntity"/> and have a parameterless constructor.
/// </typeparam>
public record QueryResult<TEntity> where TEntity : IReportEntity, new() {
    /// <summary>
    /// Gets the collection of entities returned by the query.
    /// </summary>
    public IReadOnlyCollection<TEntity> Data { get; init; } = [];
}