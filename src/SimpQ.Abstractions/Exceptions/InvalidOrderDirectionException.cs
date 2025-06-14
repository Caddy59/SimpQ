namespace SimpQ.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid sort direction is specified for a column in an order clause.
/// </summary>
public class InvalidOrderDirectionException : Exception {
    /// <summary>
    /// Gets the name of the invalid column that caused the exception.
    /// </summary>
    public string? Column { get; private init; }

    /// <summary>
    /// Gets the invalid sort direction that was specified.
    /// </summary>
    public string? Direction { get; private init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOrderDirectionException"/> class
    /// with a message indicating the invalid direction used for the specified column.
    /// </summary>
    /// <param name="column">The name of the column in the order clause.</param>
    /// <param name="direction">The invalid sort direction provided (e.g., "ascending").</param>
    public InvalidOrderDirectionException(string column, string direction)
        : base($"Invalid direction for column {column} in order clause: '{direction}'.") {
        Column = column;
        Direction = direction;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOrderDirectionException"/> class.
    /// </summary>
    public InvalidOrderDirectionException() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOrderDirectionException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The error message that describes the issue.</param>
    public InvalidOrderDirectionException(string message) : base(message) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOrderDirectionException"/> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that describes the issue.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public InvalidOrderDirectionException(string message, Exception innerException) : base(message, innerException) {
    }
}