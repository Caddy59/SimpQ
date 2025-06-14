using SimpQ.Abstractions.Exceptions;

namespace SimpQ.Abstractions.UnitTests.Exceptions;

public class InvalidOrderDirectionExceptionTests {
    [Fact]
    public void Constructor_ShouldNotSetProperties_WhenUsingParameterlessOverload() {
        // Act
        var ex = new InvalidOrderDirectionException();

        // Assert
        Assert.Null(ex.Column);
        Assert.Null(ex.Direction);
        Assert.NotNull(ex.Message);
    }

    [Fact]
    public void Constructor_ShouldSetColumnAndDirection_WhenUsingColumnAndDirectionOverload() {
        // Arrange
        var column = "Amount";
        var direction = "ascendingly";

        // Act
        var ex = new InvalidOrderDirectionException(column, direction);

        // Assert
        Assert.Equal(column, ex.Column);
        Assert.Equal(direction, ex.Direction);
        Assert.Equal("Invalid direction for column Amount in order clause: 'ascendingly'.", ex.Message);
    }

    

    [Fact]
    public void Constructor_ShouldSetMessage_WhenUsingMessageOverload() {
        // Arrange
        var message = "Custom error message";

        // Act
        var ex = new InvalidOrderDirectionException(message);

        // Assert
        Assert.Equal(message, ex.Message);
        Assert.Null(ex.Column);
        Assert.Null(ex.Direction);
    }

    [Fact]
    public void Constructor_ShouldSetMessageAndInnerException_WhenUsingMessageAndInnerExceptionOverload() {
        // Arrange
        var message = "Another error";
        var inner = new InvalidOperationException("Inner");

        // Act
        var ex = new InvalidOrderDirectionException(message, inner);

        // Assert
        Assert.Equal(message, ex.Message);
        Assert.Equal(inner, ex.InnerException);
        Assert.Null(ex.Column);
        Assert.Null(ex.Direction);
    }
}