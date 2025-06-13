using SimpQ.Core.Queries;
using System.Data;
using System.Text.Json;

namespace SimpQ.SqlServer.UnitTests.Queries.OperatorHandlers;

public class DefaultOperatorHandlerTests {
    private readonly DefaultOperatorHandler _handler = new(new SimpQOperator());

    [Theory]
    [InlineData("equals")]
    [InlineData("not_equals")]
    [InlineData("greater")]
    [InlineData("less")]
    [InlineData("greater_equals")]
    [InlineData("less_equals")]
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
    [InlineData("equals", "=")]
    [InlineData("not_equals", "<>")]
    [InlineData("greater", ">")]
    [InlineData("greater_equals", ">=")]
    [InlineData("less", "<")]
    [InlineData("less_equals", "<=")]
    public void BuildClause_ShouldReturnValidSqlClause_ForValidOperator(string @operator, string sqlOperator) {
        // Arrange
        var columnName = "Age";
        var dbType = (int)SqlDbType.Int;
        var parameterContext = new ParameterContext();

        var json = JsonSerializer.SerializeToElement(30);

        // Act
        var result = _handler.BuildClause(columnName, dbType, @operator, json, parameterContext);

        // Assert
        Assert.NotNull(result);
        Assert.Equal($"[{columnName}] {sqlOperator} @p0", result);
        Assert.Equal(30, Convert.ToInt32(parameterContext.Parameters.ElementAt(0).Value!.ToString()));
    }

    [Theory]
    [InlineData("Example text")]
    [InlineData(35.8D)]
    [InlineData(true)]
    [InlineData(false)]
    public void BuildClause_ShouldReturnValidSqlClause_ForValidValueType(object value) {
        // Arrange
        var columnName = "Description";
        var dbType = (int)SqlDbType.VarChar;
        var @operator = "equals";
        var parameterContext = new ParameterContext();

        var json = JsonSerializer.SerializeToElement(value);

        // Act
        var result = _handler.BuildClause(columnName, dbType, @operator, json, parameterContext);

        // Assert
        Assert.Contains("[Description]", result);
    }

    [Fact]
    public void BuildClause_ShouldThrowArgumentException_ForUnsupportedValueType() {
        // Arrange
        var columnName = "Description";
        var dbType = (int)SqlDbType.VarChar;
        var @operator = "equals";
        var parameterContext = new ParameterContext();

        var json = JsonSerializer.SerializeToElement(new object[] { new { } });

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _handler.BuildClause(columnName, dbType, @operator, json, parameterContext));
    }
}