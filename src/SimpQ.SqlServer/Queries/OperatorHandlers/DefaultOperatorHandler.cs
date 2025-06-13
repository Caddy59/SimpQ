using SimpQ.Core.Contexts;
using System.Text.Json;

namespace SimpQ.SqlServer.Queries.OperatorHandlers;

/// <summary>
/// Handles basic comparison operators for building SQL WHERE clause fragments.
/// Supported operators include equality and inequality comparisons such as <c>equals</c>, <c>greater</c>, and <c>less</c>.
/// </summary>
/// <param name="simpQOperator">
/// The SimpQ operator definition used to resolve canonical comparison operator names.
/// </param>
public class DefaultOperatorHandler(SimpQOperator simpQOperator) : IWhereOperatorHandler {
    private readonly HashSet<string> _allowedOperators = [
        simpQOperator.Equals,
        simpQOperator.NotEquals,
        simpQOperator.GreaterThanOrEqual,
        simpQOperator.LessThanOrEqual,
        simpQOperator.GreaterThan,
        simpQOperator.LessThan
    ];

    /// <inheritdoc />
    public bool CanHandle(string @operator) => _allowedOperators.Contains(@operator);

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    /// Thrown when the JSON value is of an unsupported type (e.g., object, array, null).
    /// </exception>
    public string BuildClause(string columnName, int dbType, string @operator, JsonElement value, ParameterContext parameterContext) {
        object paramValue = value.ValueKind switch {
            JsonValueKind.Number => value.GetDecimal(),
            JsonValueKind.String => value.GetString()!,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => throw new ArgumentException("Unsupported value type.")
        };

        var paramName = parameterContext.Add(paramValue, dbType);
        var sqlOperator = SqlServerAllowedOperator.ComparisonOperators[@operator];
        return $"{columnName.EscapeColumnName()} {sqlOperator} {paramName}";
    }
}