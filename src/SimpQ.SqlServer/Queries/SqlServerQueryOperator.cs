namespace SimpQ.SqlServer.Queries;

/// <summary>
/// Represents SQL Server-compatible implementations of all supported query operators.
/// This class maps logical operator names used in filtering logic to actual SQL syntax.
/// </summary>
public class SqlServerQueryOperator : IQueryOperator {
    public string IsNull => "IS NULL";
    public string IsNotNull => "IS NOT NULL";
    public new string Equals => "=";
    public string NotEquals => "<>";
    public string GreaterThanOrEqual => ">=";
    public string LessThanOrEqual => "<=";
    public string GreaterThan => ">";
    public string LessThan => "<";
    public string Like => "LIKE";
    public string NotLike => "NOT LIKE";
    public string StartsWith => "LIKE";
    public string EndsWith => "LIKE";
    public string In => "IN";
    public string NotIn => "NOT IN";
    public string Between => "BETWEEN";
    public string NotBetween => "NOT BETWEEN";
    public string And => "AND";
    public string Or => "OR";
    public string Ascending => "ASC";
    public string Descending => "DESC";
}