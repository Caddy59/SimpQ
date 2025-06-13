namespace SimpQ.Abstractions.Models.Requests;

/// <summary>
/// Represents a single field to be selected in a query.
/// </summary>
public record Select {
    /// <summary>
    /// Gets or sets the name of the field (column) to include in select.
    /// </summary>
    public string Field { get; set; } = default!;
}