namespace SimpQ.Core.UnitTests.Mocks;

internal class MockQueryOperatorValue : IQueryOperator {
    public new string Equals => "=";
    public string NotEquals => "<>";
    public string IsNull => "IS NULL";
    public string IsNotNull => "IS NOT NULL";
    public string GreaterThanOrEqual => ">=";
    public string LessThanOrEqual => "<=";
    public string GreaterThan => ">";
    public string LessThan => "<";
    public string Like => "LIKE";
    public string NotLike => "NOT LIKE";
    public string StartsWith => "START";
    public string EndsWith => "END";
    public string In => "IN";
    public string NotIn => "NOT IN";
    public string Between => "BETWEEN";
    public string NotBetween => "NOT BETWEEN";
    public string And => "AND";
    public string Or => "OR";
    public string Ascending => "ASC";
    public string Descending => "DESC";
}