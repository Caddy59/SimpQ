namespace SimpQ.SqlServer.UnitTests.Queries.ClauseBuilders;

public class PaginationClauseBuilderTests {
    [Theory]
    [InlineData(0, 10, "OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;")]
    [InlineData(25, 50, "OFFSET 25 ROWS FETCH NEXT 50 ROWS ONLY;")]
    public void Build_ShouldReturnValidSqlClause_WhenInputIsValid(int offset, int pageSize, string expected) {
        // Act
        var result = PaginationClauseBuilder.Build(offset, pageSize);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Build_ShouldThrowArgumentOutOfRangeException_WhenOffsetIsNegative() {
        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => PaginationClauseBuilder.Build(-1, 10));
        Assert.Equal("offset", ex.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Build_ShouldThrowArgumentOutOfRangeException_WhenPageSizeIsZeroOrNegative(int invalidPageSize) {
        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => PaginationClauseBuilder.Build(0, invalidPageSize));
        Assert.Equal("pageSize", ex.ParamName);
    }
}