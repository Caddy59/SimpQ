using SimpQ.Abstractions.Exceptions;

namespace SimpQ.Abstractions.UnitTests.Exceptions;

public class InvalidColumExceptionTests {
    [Fact]
    public void Constructor_ShouldNotSetProperties_WhenUsingParameterlessOverload() {
        // Act
        var ex = new InvalidColumException();

        // Assert
        Assert.Null(ex.Column);
        Assert.Null(ex.Clause);
        Assert.Null(ex.InnerException);
        Assert.NotNull(ex.Message);
    }

    [Fact]
    public void Constructor_ShouldSetColumnAndClause_WhenUsingColumnAndClauseOverload() {
        // Arrange
        var column = "TestColumn";
        var clause = "where";

        // Act
        var ex = new InvalidColumException(column, clause);

        // Assert
        Assert.Equal(column, ex.Column);
        Assert.Equal(clause, ex.Clause);
        Assert.Equal("Invalid column in where clause: 'TestColumn'.", ex.Message);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void Constructor_ShouldSetMessage_WhenUsingMessageOverload() {
        // Arrange
        var message = "Custom error message";

        // Act
        var ex = new InvalidColumException(message);

        // Assert
        Assert.Equal(message, ex.Message);
        Assert.Null(ex.Column);
        Assert.Null(ex.Clause);
    }

    [Fact]
    public void Constructor_ShouldSetMessageAndInnerException_WhenUsingMessageAndInnerExceptionOverload() {
        // Arrange
        var message = "Another error";
        var inner = new InvalidOperationException("Inner");

        // Act
        var ex = new InvalidColumException(message, inner);

        // Assert
        Assert.Equal(message, ex.Message);
        Assert.Equal(inner, ex.InnerException);
        Assert.Null(ex.Column);
        Assert.Null(ex.Clause);
    }
}