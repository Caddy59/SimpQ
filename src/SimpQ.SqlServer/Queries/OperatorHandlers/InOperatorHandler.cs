using SimpQ.Core.Contexts;
using System.Text.Json;

namespace SimpQ.SqlServer.Queries.OperatorHandlers;

/// <summary>
/// Handles <c>in</c> and <c>not_in</c> operators to generate SQL WHERE clauses that compare against a list of values.
/// </summary>
/// <param name="simpQOperator">
/// The SimpQ operator definition used to resolve canonical collection-based operator names.
/// </param>
public class InOperatorHandler(SimpQOperator simpQOperator) : IWhereOperatorHandler {
    private readonly HashSet<string> _allowedOperators = [
        simpQOperator.In,
        simpQOperator.NotIn
    ];

    /// <inheritdoc />
    public bool CanHandle(string @operator) => _allowedOperators.Contains(@operator);

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    /// Thrown if the provided value is not a JSON array.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if any array element is of an unsupported type (e.g., object, null).
    /// </exception>
    public string BuildClause(string columnName, int dbType, string @operator, JsonElement value, ParameterContext parameterContext) {
        if (value.ValueKind is not JsonValueKind.Array)
            throw new InvalidOperationException($"'{@operator}' operator requires an array of values.");

        var paramNames = new List<string>();
        foreach (var element in value.EnumerateArray()) {
            object paramValue = element.ValueKind switch {
                JsonValueKind.Number => element.GetDecimal(),
                JsonValueKind.String => element.GetString()!,
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => throw new ArgumentException("Unsupported array element type.")
            };

            var paramName = parameterContext.Add(paramValue, dbType);
            paramNames.Add(paramName);
        }

        var paramList = string.Join(", ", paramNames);
        var sqlOperator = SqlServerAllowedOperator.ComparisonOperators[@operator];
        return $"{columnName.EscapeColumnName()} {sqlOperator} ({paramList})";
    }
}