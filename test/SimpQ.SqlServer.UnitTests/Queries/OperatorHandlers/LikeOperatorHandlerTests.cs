using SimpQ.Core.Queries;
using SimpQ.SqlServer.Queries;
using System.Data;
using System.Text.Json;

namespace SimpQ.SqlServer.UnitTests.Queries.OperatorHandlers; 
public class LikeOperatorHandlerTests {
    private readonly LikeOperatorHandler _handler = new(new SimpQOperator());

    [Theory]
    [InlineData("contains")]
    [InlineData("not_contains")]
    [InlineData("starts_with")]
    [InlineData("ends_with")]
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
    [InlineData("contains", "%Test%")]
    [InlineData("not_contains", "%Test%")]
    [InlineData("starts_with", "Test%")]
    [InlineData("ends_with", "%Test")]
    public void BuildClause_ShouldReturnValidSqlClause_ForValidOperator(string @operator, string expectedPattern) {
        // Arrange
        var columnName = "Description";
        var dbType = (int)SqlDbType.VarChar;
        var parameterContext = new ParameterContext();

        var json = JsonSerializer.SerializeToElement("Test");

        // Act
        var result = _handler.BuildClause(columnName, dbType, @operator, json, parameterContext);

        // Assert
        Assert.NotNull(result);
        Assert.Contains($"[{columnName}] {SqlServerAllowedOperator.ComparisonOperators[@operator]} @p0 ESCAPE '\\'", result);
        Assert.Single(parameterContext.Parameters);
        Assert.Equal(expectedPattern, parameterContext.Parameters.First().Value);
    }

    [Fact]
    public void BuildClause_ShouldEscapeSpecialCharacters_InValue() {
        // Arrange
        var columnName = "Description";
        var dbType = (int)SqlDbType.VarChar;
        var @operator = "contains";
        var parameterContext = new ParameterContext();

        var json = JsonSerializer.SerializeToElement("Test%_[]");

        // Act
        var result = _handler.BuildClause(columnName, dbType, @operator, json, parameterContext);

        // Assert
        Assert.NotNull(result);
        Assert.Contains($"[{columnName}] {SqlServerAllowedOperator.ComparisonOperators[@operator]} @p0 ESCAPE '\\'", result);
        Assert.Single(parameterContext.Parameters);
        Assert.Equal(@"%Test\%\_\[]%", parameterContext.Parameters.First().Value!.ToString());
    }

    [Fact]
    public void BuildClause_ShouldThrowArgumentException_WhenValueIsNotString() {
        // Arrange
        var columnName = "Name";
        var dbType = (int)SqlDbType.VarChar;
        var @operator = "contains";
        var parameterContext = new ParameterContext();

        var json = JsonSerializer.SerializeToElement(123);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _handler.BuildClause(columnName, dbType, @operator, json, parameterContext));
    }
}