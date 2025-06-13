using SimpQ.Abstractions.Attributes.Operators;

namespace SimpQ.Abstractions.Queries;

/// <summary>
/// Defines the set of supported string representations for query operators used in filtering, 
/// logical grouping, and sorting within query building.
/// </summary>
public interface IQueryOperator {
    [ComparisonOperator]
    string IsNull { get; }
    [ComparisonOperator]
    string IsNotNull { get; }
    [ComparisonOperator]
    string Equals { get; }
    [ComparisonOperator]
    string NotEquals { get; }
    [ComparisonOperator]
    string GreaterThanOrEqual { get; }
    [ComparisonOperator]
    string LessThanOrEqual { get; }
    [ComparisonOperator]
    string GreaterThan { get; }
    [ComparisonOperator]
    string LessThan { get; }
    [ComparisonOperator]
    string Like { get; }
    [ComparisonOperator]
    string NotLike { get; }
    [ComparisonOperator]
    string StartsWith { get; }
    [ComparisonOperator]
    string EndsWith { get; }
    [ComparisonOperator]
    string In { get; }
    [ComparisonOperator]
    string NotIn { get; }
    [ComparisonOperator]
    string Between { get; }
    [ComparisonOperator]
    string NotBetween { get; }
    [LogicalOperator]
    string And { get; }
    [LogicalOperator]
    string Or { get; }
    [OrderingOperator]
    string Ascending { get; }
    [OrderingOperator]
    string Descending { get; }
}