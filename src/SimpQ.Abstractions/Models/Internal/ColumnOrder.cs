using SimpQ.Abstractions.Enums;

namespace SimpQ.Abstractions.Models.Internal;

/// <summary>
/// Represents ordering metadata for a specific column, including its sort priority and direction.
/// </summary>
/// <param name="Name">The name of the column to apply ordering to.</param>
/// <param name="Priority">The priority of this order relative to others; lower values are applied first.</param>
/// <param name="Direction">The direction in which to sort the column (ascending or descending).</param>
public record ColumnOrder(string Name, int Priority, OrderDirection Direction);