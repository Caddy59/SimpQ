using SimpQ.Core.Queries;
using System.Data;
using System.Text.Json;

namespace SimpQ.SqlServer.UnitTests.Queries.OperatorHandlers;

public class InOperatorHandlerTests {
    private readonly InOperatorHandler _handler = new(new SimpQOperator());

    [Theory]
    [InlineData("in")]
    [InlineData("not_in")]
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
    [InlineData("in", "IN")]
    [InlineData("not_in", "NOT IN")]
    public void BuildClause_ShouldReturnValidSqlClause_ForValidOperator(string @operator, string sqlOperator) {
        // Arrange
        var columnName = "Age";
        var dbType = (int)SqlDbType.Int;
        var parameterContext = new ParameterContext();

        var json = JsonSerializer.SerializeToElement(new[] { 18, 30 });

        // Act
        var result = _handler.BuildClause(columnName, dbType, @operator, json, parameterContext);

        // Assert
        Assert.NotNull(result);
        Assert.Equal($"[{columnName}] {sqlOperator} (@p0, @p1)", result);
        Assert.Equal(2, parameterContext.Parameters.Count);
        Assert.Equal(18, Convert.ToInt32(parameterContext.Parameters.ElementAt(0).Value!.ToString()));
        Assert.Equal(30, Convert.ToInt32(parameterContext.Parameters.ElementAt(1).Value!.ToString()));
    }

    [Theory]
    [InlineData("A", "Z")]
    [InlineData(35.8D, 40.7D)]
    [InlineData(true, false)]
    public void BuildClause_ShouldReturnValidSqlClause_ForValidValueType(params object[] value) {
        // Arrange
        var columnName = "Description";
        var dbType = (int)SqlDbType.VarChar;
        var @operator = "in";
        var parameterContext = new ParameterContext();

        var json = JsonSerializer.SerializeToElement(value);

        // Act
        var result = _handler.BuildClause(columnName, dbType, @operator, json, parameterContext);

        // Assert
        Assert.Contains("[Description]", result);
    }

    [Fact]
    public void BuildClause_ShouldThrowInvalidOperationException_WhenValueIsNotArray() {
        // Arrange
        var columnName = "Age";
        var dbType = (int)SqlDbType.Int;
        var @operator = "in";
        var parameterContext = new ParameterContext();

        var json = JsonSerializer.SerializeToElement(18);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _handler.BuildClause(columnName, dbType, @operator, json, parameterContext));
    }

    [Fact]
    public void BuildClause_ShouldThrowArgumentException_ForInvalidValueType() {
        // Arrange
        var columnName = "Age";
        var dbType = (int)SqlDbType.Int;
        var @operator = "in";
        var parameterContext = new ParameterContext();

        var json = JsonSerializer.SerializeToElement(new object[] { new { }, 30 });

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _handler.BuildClause(columnName, dbType, @operator, json, parameterContext));
    }
}