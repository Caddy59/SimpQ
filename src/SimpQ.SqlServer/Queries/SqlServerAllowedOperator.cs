using System.Collections.Frozen;

namespace SimpQ.SqlServer.Queries;

/// <summary>
/// Provides mappings of allowed query operators for SQL Server,
/// resolving user-facing operator keywords to SQL-compatible equivalents.
/// </summary>
public static class SqlServerAllowedOperator {
    /// <summary>
    /// Gets a frozen dictionary of comparison operators (e.g., "equals", "greater_than") mapped to their SQL Server equivalents (e.g., "=", "&gt;").
    /// </summary>
    public static FrozenDictionary<string, string> ComparisonOperators => OperatorHelper.GetComparisonOperators<SimpQOperator, SqlServerQueryOperator>();

    /// <summary>
    /// Gets a frozen dictionary of logical operators (e.g., "and", "or") mapped to their SQL Server equivalents (e.g., "AND", "OR").
    /// </summary>
    public static FrozenDictionary<string, string> LogicalOperators => OperatorHelper.GetLogicalOperators<SimpQOperator, SqlServerQueryOperator>();

    /// <summary>
    /// Gets a frozen dictionary of ordering operators (e.g., "asc", "desc") mapped to their SQL Server equivalents ("ASC", "DESC").
    /// </summary>
    public static FrozenDictionary<string, string> OrderingOperators => OperatorHelper.GetOrderingOperators<SimpQOperator, SqlServerQueryOperator>();
}