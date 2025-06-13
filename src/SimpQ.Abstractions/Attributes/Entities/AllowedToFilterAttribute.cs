namespace SimpQ.Abstractions.Attributes.Entities;

/// <summary>
/// Indicates that the decorated property is eligible for filtering in queries.
/// </summary>
/// <remarks>
/// This attribute is used by query generation tools to identify which properties can be used in filter expressions.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class AllowedToFilterAttribute : Attribute { }