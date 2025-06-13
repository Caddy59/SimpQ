namespace SimpQ.Abstractions.Models.Requests;

/// <summary>
/// Serves as the base type for defining filtering criteria in dynamic query operations.
/// </summary>
public interface IFilter { }

/// <summary>
/// Represents a simple filtering condition that compares a specified field against a value using a given operator.
/// </summary>
public record FilterCondition : IFilter {
    /// <summary>
    /// Gets or sets the name of the field to be filtered.
    /// </summary>
    public string Field { get; set; } = default!;

    /// <summary>
    /// Gets or sets the comparison operator (e.g., equals, greater_than) used in the filter.
    /// </summary>
    public string Operator { get; set; } = default!;

    /// <summary>
    /// Gets or sets the value to be used for filtering against the specified field.
    /// </summary>
    public object Value { get; set; } = default!;
}

/// <summary>
/// Represents a composite filter that groups multiple filtering criteria using a logical operator (e.g., "and" or "or").
/// </summary>
public record FilterGroup : IFilter {
    /// <summary>
    /// Gets or sets the logical operator used to combine the filtering conditions.
    /// Typically "and" or "or".
    /// </summary>
    public string Logic { get; set; } = "and";

    /// <summary>
    /// Gets or sets the collection of filter criteria that are combined using the specified logic.
    /// This collection can contain both <see cref="FilterCondition"/> and nested <see cref="FilterGroup"/> instances.
    /// </summary>
    public IReadOnlyCollection<IFilter> Conditions { get; set; } = [];
}