using SimpQ.Core.Contexts;
using System.Text.Json;

namespace SimpQ.SqlServer.Queries.OperatorHandlers;

/// <summary>
/// Handles null-check operators such as <c>is_null</c> and <c>is_not_null</c>
/// for generating SQL WHERE clause fragments that evaluate nullability.
/// </summary>
/// <param name="simpQOperator">
/// The SimpQ operator definition used to identify the canonical null-check operator keywords.
/// </param>
public class NullCheckOperatorHandler(SimpQOperator simpQOperator) : IWhereOperatorHandler {
    private readonly HashSet<string> _allowedOperators = [
        simpQOperator.IsNull,
        simpQOperator.IsNotNull
    ];

    /// <inheritdoc />
    public bool CanHandle(string @operator) => _allowedOperators.Contains(@operator);

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    /// Thrown if a value is provided when using a null-check operator.
    /// These operators are unary and do not accept values.
    /// </exception>
    public string BuildClause(string columnName, int dbType, string @operator, JsonElement value, ParameterContext parameterContext) {
        if (value.ValueKind is not JsonValueKind.Null or JsonValueKind.Undefined)
            throw new ArgumentException($"'{@operator}' operator does not accept a value.");

        var sqlOperator = SqlServerAllowedOperator.ComparisonOperators[@operator];

        return $"{columnName.EscapeColumnName()} {sqlOperator}";
    }
}