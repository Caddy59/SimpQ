namespace SimpQ.Abstractions.Models.Requests;

/// <summary>
/// Represents an ordering instruction for a query, specifying the field to sort by and the direction.
/// </summary>
public record Order {
    /// <summary>
    /// Gets or sets the name of the field (column) to apply ordering to.
    /// </summary>
    public string Field { get; set; } = default!;

    /// <summary>
    /// Gets or sets the sort direction. Expected values are typically <c>"asc"</c> or <c>"desc"</c>.
    /// Default is <c>"asc"</c>.
    /// </summary>
    public string Direction { get; set; } = "asc";
}