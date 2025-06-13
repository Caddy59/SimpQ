namespace SimpQ.Abstractions.Exceptions;

public class InvalidValueException : Exception {
    public InvalidValueException(string value)
        : base($"Invalid value in where clause: '{value}'.") {
    }

    public InvalidValueException() {
    }

    public InvalidValueException(string message, Exception innerException) : base(message, innerException) {
    }
}