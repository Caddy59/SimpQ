namespace SimpQ.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid comparison or logical operator is used in a query clause.
/// </summary>
public class InvalidOperatorException : Exception {
    /// <summary>
    /// Gets the invalid operator that caused the exception.
    /// </summary>
    public string? Operator { get; private init; }

    /// <summary>
    /// Gets the type of operator used (e.g., "comparison", "logical").
    /// </summary>
    public string? OperatorType { get; private init; }

    /// <summary>
    /// Gets the name of the column that the operator was applied to, if applicable.
    /// </summary>
    public string? ColumnName { get; private init; }

    /// <summary>
    /// Gets the CLR type of the property associated with the column, if applicable.
    /// </summary>
    public Type? PropertyType { get; private init; }

    /// <summary>
    /// Gets a list of valid operators for the given property type, if applicable.
    /// </summary>
    public string[]? ValidOperators { get; private init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOperatorException"/> class
    /// with a message indicating the invalid operator and its type.
    /// </summary>
    /// <param name="operator">The invalid operator encountered.</param>
    /// <param name="operatorType">The type of operator (e.g., "comparison", "logical").</param>
    public InvalidOperatorException(string @operator, string operatorType)
        : base($"Invalid {operatorType} operator in where clause: '{@operator}'.") {
        Operator = @operator;
        OperatorType = operatorType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOperatorException"/> class
    /// with a detailed message including the target column, data type, and valid operators.
    /// </summary>
    /// <param name="operator">The invalid operator encountered.</param>
    /// <param name="operatorType">The type of operator (e.g., "comparison", "logical").</param>
    /// <param name="columnName">The name of the column the operator was applied to.</param>
    /// <param name="propertyType">The CLR type of the property associated with the column.</param>
    /// <param name="validOperators">A list of valid operator strings for the given property type.</param>
    public InvalidOperatorException(string @operator, string operatorType, string columnName, Type propertyType, string[] validOperators)
        : base($"Invalid comparison operator '{@operator}' applied to column '{columnName}' in where clause. Valid operators for '{propertyType.Name}': '{string.Join(", ", validOperators)}'.") {
        Operator = @operator;
        OperatorType = operatorType;
        ColumnName = columnName;
        PropertyType = propertyType;
        ValidOperators = validOperators;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOperatorException"/> class.
    /// </summary>
    public InvalidOperatorException() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOperatorException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The error message that describes the issue.</param>
    public InvalidOperatorException(string message) : base(message) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOperatorException"/> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that describes the issue.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public InvalidOperatorException(string message, Exception innerException) : base(message, innerException) {
    }
}