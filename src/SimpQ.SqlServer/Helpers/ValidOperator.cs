using System.Collections.Frozen;

namespace SimpQ.SqlServer.Helpers;

/// <summary>
/// Provides validation logic for determining which comparison operators are valid for a given .NET type,
/// based on a configured <see cref="SimpQOperator"/> definition.
/// </summary>
public class ValidOperator {
    private readonly FrozenDictionary<string, string[]> _comparisonOperators;
    private readonly SimpQOperator _queryOperator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidOperator"/> class and precomputes 
    /// the valid operators for supported .NET primitive types.
    /// </summary>
    /// <param name="queryOperator">The operator definition used to resolve supported values for each type.</param>
    public ValidOperator(SimpQOperator queryOperator) {
        _queryOperator = queryOperator;
        var validOperatorsForText = GetValidOperatorsForText();
        var validOperatorsForBoolean = GetValidOperatorsForBoolean();
        var validOperatorsForGuid = GetValidOperatorsForGuid();
        var validOperatorsForDate = GetValidOperatorsForDate();
        var validOperatorsForNumber = GetValidOperatorsForNumber();

        _comparisonOperators = new Dictionary<string, string[]>() {
            { nameof(String), validOperatorsForText },
            { nameof(Char), validOperatorsForText },
            { nameof(Boolean), validOperatorsForBoolean },
            { nameof(Guid), validOperatorsForGuid },
            { nameof(DateTime), validOperatorsForDate },
            { nameof(DateTimeOffset), validOperatorsForDate },
            { nameof(DateOnly), validOperatorsForDate },
            { nameof(TimeOnly), validOperatorsForDate },
            { nameof(Byte), validOperatorsForNumber },
            { nameof(SByte), validOperatorsForNumber},
            { nameof(Int16), validOperatorsForNumber },
            { nameof(UInt16), validOperatorsForNumber },
            { nameof(Int32), validOperatorsForNumber },
            { nameof(UInt32), validOperatorsForNumber },
            { nameof(Int64), validOperatorsForNumber },
            { nameof(UInt64), validOperatorsForNumber },
            { nameof(Int128), validOperatorsForNumber },
            { nameof(UInt128), validOperatorsForNumber },
            { nameof(Single), validOperatorsForNumber },
            { nameof(Double), validOperatorsForNumber },
            { nameof(Decimal), validOperatorsForNumber }
        }.ToFrozenDictionary();
    }

    /// <summary>
    /// Determines whether a given operator is valid for the specified .NET type.
    /// </summary>
    /// <param name="type">The data type to check (e.g., typeof(string), typeof(DateTime)).</param>
    /// <param name="operator">The operator string to validate (e.g., "equals", "between").</param>
    /// <returns><c>true</c> if the operator is allowed for the type; otherwise, <c>false</c>.</returns>
    public bool IsOperatorValidForType(Type type, string @operator) =>
        _comparisonOperators.TryGetValue(type.Name, out var validOperators) && validOperators.Contains(@operator);

    /// <summary>
    /// Gets all valid operators for the specified .NET type.
    /// </summary>
    /// <param name="type">The data type to retrieve operators for.</param>
    /// <returns>An array of operator strings valid for the specified type. Returns an empty array if the type is not supported.</returns>
    public string[] GetOperatorsForType(Type type) {
        _comparisonOperators.TryGetValue(type.Name, out var validOperators);
        return validOperators ?? [];
    }

    /// <summary>
    /// Returns the set of valid comparison operators for text-based types such as <see cref="string"/> or <see cref="char"/>.
    /// </summary>
    private string[] GetValidOperatorsForText() => [
        _queryOperator.IsNull,
        _queryOperator.IsNotNull,
        _queryOperator.Equals,
        _queryOperator.NotEquals,
        _queryOperator.Like,
        _queryOperator.NotLike,
        _queryOperator.StartsWith,
        _queryOperator.EndsWith,
        _queryOperator.LessThan,
        _queryOperator.LessThanOrEqual,
        _queryOperator.GreaterThanOrEqual,
        _queryOperator.GreaterThan,
        _queryOperator.In,
        _queryOperator.NotIn
    ];

    /// <summary>
    /// Returns the set of valid comparison operators for boolean values.
    /// </summary>
    private string[] GetValidOperatorsForBoolean() => [
        _queryOperator.IsNull,
        _queryOperator.IsNotNull,
        _queryOperator.Equals,
        _queryOperator.NotEquals
    ];

    /// <summary>
    /// Returns the set of valid comparison operators for <see cref="Guid"/> values.
    /// </summary>
    private string[] GetValidOperatorsForGuid() => [
        _queryOperator.IsNull,
        _queryOperator.IsNotNull,
        _queryOperator.Equals,
        _queryOperator.NotEquals,
        _queryOperator.LessThan,
        _queryOperator.LessThanOrEqual,
        _queryOperator.GreaterThanOrEqual,
        _queryOperator.GreaterThan,
        _queryOperator.In,
        _queryOperator.NotIn
    ];

    /// <summary>
    /// Returns the set of valid comparison operators for date and time types (e.g., <see cref="DateTime"/>, <see cref="DateOnly"/>).
    /// </summary>
    private string[] GetValidOperatorsForDate() => [
        _queryOperator.IsNull,
        _queryOperator.IsNotNull,
        _queryOperator.Equals,
        _queryOperator.NotEquals,
        _queryOperator.LessThan,
        _queryOperator.LessThanOrEqual,
        _queryOperator.GreaterThanOrEqual,
        _queryOperator.GreaterThan,
        _queryOperator.In,
        _queryOperator.NotIn,
        _queryOperator.Between,
        _queryOperator.NotBetween
    ];

    /// <summary>
    /// Returns the set of valid comparison operators for numeric types (e.g., <see cref="int"/>, <see cref="decimal"/>).
    /// </summary>
    private string[] GetValidOperatorsForNumber() => [
        _queryOperator.IsNull,
        _queryOperator.IsNotNull,
        _queryOperator.Equals,
        _queryOperator.NotEquals,
        _queryOperator.LessThan,
        _queryOperator.LessThanOrEqual,
        _queryOperator.GreaterThanOrEqual,
        _queryOperator.GreaterThan,
        _queryOperator.In,
        _queryOperator.NotIn,
        _queryOperator.Between,
        _queryOperator.NotBetween
    ];
}