namespace SimpQ.Abstractions.Attributes.Operators;

/// <summary>
/// Indicates that the decorated property specifies the sorting direction
/// (e.g., "ascending", "descending") to apply to an ordering expression in a query.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class OrderingOperatorAttribute : Attribute { }