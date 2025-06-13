using SimpQ.Abstractions.Queries;

namespace SimpQ.Core.Queries;

/// <summary>
/// Provides the canonical set of operator keywords used in SimpQ's query input layer.
/// This default implementation of <see cref="IQueryOperator"/> defines standardized, user-facing operator tokens
/// that are later translated to provider-specific equivalents (e.g., SQL Server).
/// </summary>
public class SimpQOperator : IQueryOperator {
    public string IsNull => "is_null";
    public string IsNotNull => "is_not_null";
    public new string Equals => "equals";
    public string NotEquals => "not_equals";
    public string GreaterThanOrEqual => "greater_equals";
    public string LessThanOrEqual => "less_equals";
    public string GreaterThan => "greater";
    public string LessThan => "less";
    public string Like => "contains";
    public string NotLike => "not_contains";
    public string StartsWith => "starts_with";
    public string EndsWith => "ends_with";
    public string In => "in";
    public string NotIn => "not_in";
    public string Between => "between";
    public string NotBetween => "not_between";
    public string And => "and";
    public string Or => "or";
    public string Ascending => "asc";
    public string Descending => "desc";
}