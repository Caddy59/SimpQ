namespace SimpQ.Abstractions.Attributes.Entities;

/// <summary>
/// Indicates that the decorated property can be used for ordering in queries.
/// </summary>
/// <remarks>
/// This attribute is used by query generation tools to identify which properties can be used in order expressions.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class AllowedToOrderAttribute : Attribute { }