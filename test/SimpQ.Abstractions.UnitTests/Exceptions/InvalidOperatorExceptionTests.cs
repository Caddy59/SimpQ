using SimpQ.Abstractions.Exceptions;

namespace SimpQ.Abstractions.UnitTests.Exceptions;

public class InvalidOperatorExceptionTests {
    [Fact]
    public void Constructor_ShouldNotSetProperties_WhenUsingParameterlessOverload() {
        // Act
        var ex = new InvalidOperatorException();

        // Assert
        Assert.Null(ex.Operator);
        Assert.Null(ex.OperatorType);
        Assert.Null(ex.ColumnName);
        Assert.Null(ex.PropertyType);
        Assert.Null(ex.ValidOperators);
        Assert.NotNull(ex.Message);
    }

    [Fact]
    public void Constructor_ShouldSetOperatorAndOperatorType_WhenUsingOperatorAndOperatorTypeOverload() {
        // Arrange
        var @operator = "!=";
        var operatorType = "comparison";

        // Act
        var ex = new InvalidOperatorException(@operator, operatorType);

        // Assert
        Assert.Equal(@operator, ex.Operator);
        Assert.Equal(operatorType, ex.OperatorType);
        Assert.Null(ex.ColumnName);
        Assert.Null(ex.PropertyType);
        Assert.Null(ex.ValidOperators);
        Assert.Equal("Invalid comparison operator in where clause: '!='.", ex.Message);
    }

    [Fact]
    public void Constructor_ShouldSetAllDetails_WhenUsingAllDetailsOverload() {
        // Arrange
        var @operator = ">";
        var operatorType = "comparison";
        var column = "Name";
        var type = typeof(string);
        var validOperators = new[] { "=", "!=" };

        // Act
        var ex = new InvalidOperatorException(@operator, operatorType, column, type, validOperators);

        // Assert
        Assert.Equal(@operator, ex.Operator);
        Assert.Equal(operatorType, ex.OperatorType);
        Assert.Equal(column, ex.ColumnName);
        Assert.Equal(type, ex.PropertyType);
        Assert.Equal(validOperators, ex.ValidOperators);
        Assert.Equal("Invalid comparison operator '>' applied to column 'Name' in where clause. Valid operators for 'String': '=, !='.", ex.Message);
    }

    [Fact]
    public void Constructor_ShouldSetMessage_WhenUsingMessageOverload() {
        // Arrange
        var message = "Custom error message";

        // Act
        var ex = new InvalidOperatorException(message);

        // Assert
        Assert.Equal(message, ex.Message);
        Assert.Null(ex.Operator);
        Assert.Null(ex.OperatorType);
        Assert.Null(ex.ColumnName);
        Assert.Null(ex.PropertyType);
        Assert.Null(ex.ValidOperators);
    }

    [Fact]
    public void Constructor_ShouldSetMessageAndInnerException_WhenUsingMessageAndInnerExceptionOverload() {
        // Arrange
        var message = "Another error";
        var inner = new InvalidOperationException("Inner");

        // Act
        var ex = new InvalidOperatorException(message, inner);

        // Assert
        Assert.Equal(message, ex.Message);
        Assert.Equal(inner, ex.InnerException);
        Assert.Null(ex.Operator);
        Assert.Null(ex.OperatorType);
        Assert.Null(ex.ColumnName);
        Assert.Null(ex.PropertyType);
        Assert.Null(ex.ValidOperators);
    }
}