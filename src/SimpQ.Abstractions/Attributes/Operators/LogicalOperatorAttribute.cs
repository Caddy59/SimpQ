namespace SimpQ.Abstractions.Attributes.Operators;

/// <summary>
/// Indicates that the decorated property represents a logical operator 
/// (e.g., "and", "or") to combine multiple filter conditions in a query.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class LogicalOperatorAttribute : Attribute { }