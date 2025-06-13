namespace SimpQ.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid column name is used in a query clause.
/// </summary>
public class InvalidColumException : Exception {
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidColumException"/> class
    /// with a generated message that includes the invalid column and the clause it was found in.
    /// </summary>
    /// <param name="column">The name of the invalid column.</param>
    /// <param name="clause">The clause where the column was used (e.g., select, where, order).</param>
    public InvalidColumException(string column, string clause)
        : base($"Invalid column in {clause} clause: '{column}'.") {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidColumException"/> class.
    /// </summary>
    public InvalidColumException() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidColumException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidColumException(string message) : base(message) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidColumException"/> class with a specified 
    /// error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public InvalidColumException(string message, Exception innerException) : base(message, innerException) {
    }
}