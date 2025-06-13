namespace SimpQ.Abstractions.Models.Requests;

/// <summary>
/// Represents a set of query parameters, including selected fields, filters, and ordering.
/// </summary>
public record QueryParams {
    /// <summary>
    /// Gets or sets the list of fields to include in select.
    /// </summary>
    public List<Select>? Select { get; set; }

    /// <summary>
    /// Gets or sets the root filter group that defines the filtering logic (e.g., conditions and logical operators).
    /// </summary>
    public FilterGroup? Filters { get; set; }

    /// <summary>
    /// Gets or sets the list of ordering instructions to apply to the query.
    /// </summary>
    public List<Order>? Order { get; set; }
}