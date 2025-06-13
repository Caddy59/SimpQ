using SimpQ.Core.Contexts;
using System.Text.Json;

namespace SimpQ.SqlServer.Queries.OperatorHandlers;

/// <summary>
/// Handles SQL pattern-matching operators such as <c>contains</c>, <c>not_contains</c>,
/// <c>starts_with</c>, and <c>ends_with</c>, using SQL Server's <c>LIKE</c> and <c>NOT LIKE</c>.
/// </summary>
public class LikeOperatorHandler : IWhereOperatorHandler {
    private readonly SimpQOperator _simpQOperator;
    private readonly HashSet<string> _allowedOperators;

    /// <summary>
    /// Initializes a new instance of the <see cref="LikeOperatorHandler"/> class
    /// and registers the supported text-pattern operators (e.g., <c>contains</c>, <c>starts_with</c>, <c>ends_with</c>).
    /// </summary>
    /// <param name="simpQOperator">
    /// The SimpQ operator definition used to resolve canonical text-matching operator names.
    /// </param>
    public LikeOperatorHandler(SimpQOperator simpQOperator) {
        _simpQOperator = simpQOperator;
        _allowedOperators = [
            _simpQOperator.Like,
            _simpQOperator.NotLike,
            _simpQOperator.StartsWith,
            _simpQOperator.EndsWith
        ];
    }

    /// <inheritdoc />
    public bool CanHandle(string @operator) => _allowedOperators.Contains(@operator);

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    /// Thrown if the value is not a string or if the operator is unsupported.
    /// </exception>
    public string BuildClause(string columnName, int dbType, string @operator, JsonElement value, ParameterContext parameterContext) {
        if (value.ValueKind is not JsonValueKind.String)
            throw new ArgumentException($"'{@operator}' operator requires a string value.");

        var rawValue = value.GetString()!;
        var escapedValue = rawValue.Replace(@"\", @"\\")
            .Replace("[", @"\[")
            .Replace("%", @"\%")
            .Replace("_", @"\_");

        var sqlOperator = SqlServerAllowedOperator.ComparisonOperators[@operator];

        var pattern = @operator switch {
            var op when op == _simpQOperator.Like  => $"%{escapedValue}%",
            var op when op == _simpQOperator.NotLike => $"%{escapedValue}%",
            var op when op == _simpQOperator.StartsWith => $"{escapedValue}%",
            var op when op == _simpQOperator.EndsWith => $"%{escapedValue}",
        };

        var paramName = parameterContext.Add(pattern, dbType);
        return $@"{columnName.EscapeColumnName()} {sqlOperator} {paramName} ESCAPE '\'";
    }
}