namespace SimpQ.Core.UnitTests.Mocks;

internal class MockQueryOperatorKey : IQueryOperator {
    public new string Equals => "equals";
    public string NotEquals => "not_equals";
    public string IsNull => "is_null";
    public string IsNotNull => "is_not_null";
    public string GreaterThanOrEqual => "ge";
    public string LessThanOrEqual => "le";
    public string GreaterThan => "gt";
    public string LessThan => "lt";
    public string Like => "ct";
    public string NotLike => "not_ct";
    public string StartsWith => "str";
    public string EndsWith => "end";
    public string In => "in";
    public string NotIn => "not_in";
    public string Between => "between";
    public string NotBetween => "not_between";
    public string And => "and";
    public string Or => "or";
    public string Ascending => "asc";
    public string Descending => "desc";
}