namespace SimpQ.Abstractions.Exceptions;

public class MissingTypeOperatorsException : Exception {
    public MissingTypeOperatorsException(string columnName, Type propertyType)
        : base($"Type '{propertyType.Name}' used in column '{columnName}' has no operators mapped.") {
    }

    public MissingTypeOperatorsException() {
    }

    public MissingTypeOperatorsException(string message) : base(message) {
    }

    public MissingTypeOperatorsException(string message, Exception innerException) : base(message, innerException) {
    }
}