using SimpQ.Abstractions.Enums;

namespace SimpQ.Abstractions.Attributes.Entities;

/// <summary>
/// Identifies a property to be used as a key in keyset pagination logic.
/// </summary>
/// <remarks>
/// It is necessary to specify this attribute in at least one column of the entity when using keyset pagination.
/// </remarks>
/// <param name="priority">The priority for ordering keyset keys. Lower values are evaluated first (optional). Default is 0.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class KeysetPaginationKeyAttribute(int priority = 0) : Attribute {
    /// <summary>
    /// Gets the ordering priority of this pagination key. Lower values are applied first.
    /// </summary>
    public int Priority { get; private init; } = priority;
}