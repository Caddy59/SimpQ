using SimpQ.Core.Queries;
using System.Data;
using System.Text.Json;

namespace SimpQ.SqlServer.UnitTests.Queries.OperatorHandlers;

public class NullCheckOperatorHandlerTests {
    private readonly NullCheckOperatorHandler _handler = new(new SimpQOperator());

    [Theory]
    [InlineData("is_null")]
    [InlineData("is_not_null")]
    public void CanHandle_ShouldReturnTrue_ForValidOperator(string @operator) {
        // Act
        var result = _handler.CanHandle(@operator);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanHandle_ShouldReturnFalse_ForUnsupportedOperator() {
        // Act
        var result = _handler.CanHandle("invalid");

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("is_null", "IS NULL")]
    [InlineData("is_not_null", "IS NOT NULL")]
    public void BuildClause_ShouldReturnValidSqlClause_ForValidOperator(string @operator, string sqlOperator) {
        // Arrange
        var columnName = "Age";
        var dbType = (int)SqlDbType.Int;
        var parameterContext = new ParameterContext();

        var json = JsonSerializer.SerializeToElement((int?)null);

        // Act
        var result = _handler.BuildClause(columnName, dbType, @operator, json, parameterContext);

        // Assert
        Assert.NotNull(result);
        Assert.Equal($"[{columnName}] {sqlOperator}", result);
    }

    [Fact]
    public void BuildClause_ShouldThrowArgumentException_WhenValueIsNotNullOrUndefined() {
        // Arrange
        var columnName = "Age";
        var dbType = (int)SqlDbType.Int;
        var @operator = "is_null";
        var parameterContext = new ParameterContext();

        var json = JsonSerializer.SerializeToElement("Test");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _handler.BuildClause(columnName, dbType, @operator, json, parameterContext));
    }
}