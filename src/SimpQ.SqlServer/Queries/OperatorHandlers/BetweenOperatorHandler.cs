using SimpQ.Core.Contexts;
using System.Text.Json;

namespace SimpQ.SqlServer.Queries.OperatorHandlers;

/// <summary>
/// Handles the <c>between</c> comparison operator for building SQL WHERE clauses
/// that filter values within a specified inclusive range.
/// </summary>
public class BetweenOperatorHandler : IWhereOperatorHandler {
    private readonly SimpQOperator _simpQOperator;
    private readonly HashSet<string> _allowedOperators;

    /// <summary>
    /// Initializes a new instance of the <see cref="BetweenOperatorHandler"/> class
    /// and registers the supported range operators (e.g., <c>between</c>, <c>not_between</c>).
    /// </summary>
    /// <param name="simpQOperator">
    /// The SimpQ operator definition used to resolve canonical range-based operator names.
    /// </param>
    public BetweenOperatorHandler(SimpQOperator simpQOperator) {
        _simpQOperator = simpQOperator;
        _allowedOperators = [
            _simpQOperator.Between,
            _simpQOperator.NotBetween
        ];
    }

    /// <inheritdoc />
    public bool CanHandle(string @operator) => _allowedOperators.Contains(@operator);

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    /// Thrown when the value is not a JSON array with exactly two elements.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when one of the array values is of an unsupported JSON type.
    /// </exception>
    public string BuildClause(string columnName, int dbType, string @operator, JsonElement value, ParameterContext parameterContext) {
        if (value.ValueKind is not JsonValueKind.Array || value.GetArrayLength() != 2)
            throw new InvalidOperationException($"'{@operator}' operator requires an array with exactly two values.");

        var lower = value[0];
        var upper = value[1];

        object lowerValue = lower.ValueKind switch {
            JsonValueKind.Number => lower.GetDecimal(),
            JsonValueKind.String => lower.GetString()!,
            _ => throw new ArgumentException("Unsupported lower bound value type.")
        };

        object upperValue = upper.ValueKind switch {
            JsonValueKind.Number => upper.GetDecimal(),
            JsonValueKind.String => upper.GetString()!,
            _ => throw new ArgumentException("Unsupported upper bound value type.")
        };

        var lowerParamName = parameterContext.Add(lowerValue, dbType);
        var upperParamName = parameterContext.Add(upperValue, dbType);

        var sqlOperator = SqlServerAllowedOperator.ComparisonOperators[@operator];
        var sqlAndOperator = SqlServerAllowedOperator.LogicalOperators[_simpQOperator.And];
        return $"{columnName.EscapeColumnName()} {sqlOperator} {lowerParamName} {sqlAndOperator} {upperParamName}";
    }
}