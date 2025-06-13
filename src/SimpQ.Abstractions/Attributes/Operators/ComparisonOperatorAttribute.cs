namespace SimpQ.Abstractions.Attributes.Operators;

/// <summary>
/// Indicates that the decorated property represents a comparison operator 
/// (e.g., "equals", "less than", "contains") that will be applied to a corresponding filter value 
/// in filtering logic.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ComparisonOperatorAttribute : Attribute { }