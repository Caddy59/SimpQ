using SimpQ.Abstractions.Exceptions;

namespace SimpQ.SqlServer.UnitTests.Queries.OperatorHandlers;

public class WhereOperatorHandlerResolverTests {
    [Fact]
    public void Resolve_ShouldReturnCorrectHandler_WhenHandlerSupportsOperator() {
        // Arrange
        var mockHandler = new Mock<IWhereOperatorHandler>();
        mockHandler.Setup(h => h.CanHandle("equals")).Returns(true);

        var resolver = new WhereOperatorHandlerResolver([mockHandler.Object]);

        // Act
        var result = resolver.Resolve("equals");

        // Assert
        Assert.Equal(mockHandler.Object, result);
    }

    [Fact]
    public void Resolve_ShouldThrowNotSupportedException_WhenNoHandlerSupportsOperator() {
        // Arrange
        var mockHandler = new Mock<IWhereOperatorHandler>();
        mockHandler.Setup(h => h.CanHandle("invalid")).Returns(false);

        var resolver = new WhereOperatorHandlerResolver([mockHandler.Object]);

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => resolver.Resolve("invalid"));
    }
}