using SimpQ.Abstractions.Enums;

namespace SimpQ.Abstractions.Attributes.Entities;

/// <summary>
/// Indicates the default ordering to apply for the decorated property when no explicit order is specified in a query.
/// </summary>
/// <remarks>
/// It is necessary to specify this attribute in at least one column of the entity.
/// </remarks>
/// <param name="priority">The priority of this order relative to others, where lower means higher priority (optional). Default is 0.</param>
/// <param name="direction">The direction of the order: ascending or descending (optional). Default is ascending.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DefaultOrderAttribute(int priority = 0, OrderDirection direction = OrderDirection.Ascending) : Attribute {
    /// <summary>
    /// Gets the priority of the ordering. Lower values have higher precedence.
    /// </summary>
    public int Priority { get; private init; } = priority;
    /// <summary>
    /// Gets the default ordering direction.
    /// </summary>
    public OrderDirection OrderDirection { get; private init; } = direction;
}