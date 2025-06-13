using SimpQ.Core.Helpers;

namespace SimpQ.Core.UnitTests.Helpers;

public class OperatorHelperTests {
    [Fact]
    public void GetComparisonOperators_ShouldReturnExpectedOperators() {
        // Act
        var result = OperatorHelper.GetComparisonOperators<MockQueryOperatorKey, MockQueryOperatorValue>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(16, result.Count);
        Assert.Equal("=", result["equals"]);
        Assert.Equal("<>", result["not_equals"]);
        Assert.Equal("IS NULL", result["is_null"]);
        Assert.Equal("IS NOT NULL", result["is_not_null"]);
        Assert.Equal(">=", result["ge"]);
        Assert.Equal("<=", result["le"]);
        Assert.Equal(">", result["gt"]);
        Assert.Equal("<", result["lt"]);
        Assert.Equal("LIKE", result["ct"]);
        Assert.Equal("NOT LIKE", result["not_ct"]);
        Assert.Equal("START", result["str"]);
        Assert.Equal("END", result["end"]);
        Assert.Equal("IN", result["in"]);
        Assert.Equal("NOT IN", result["not_in"]);
        Assert.Equal("BETWEEN", result["between"]);
        Assert.Equal("NOT BETWEEN", result["not_between"]);
    }

    [Fact]
    public void GetLogicalOperators_ShouldReturnExpectedOperators() {
        // Act
        var result = OperatorHelper.GetLogicalOperators<MockQueryOperatorKey, MockQueryOperatorValue>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("AND", result["and"]);
        Assert.Equal("OR", result["or"]);
    }

    [Fact]
    public void GetOrderingOperators_ShouldReturnExpectedOperators() {
        // Act
        var result = OperatorHelper.GetOrderingOperators<MockQueryOperatorKey, MockQueryOperatorValue>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("ASC", result["asc"]);
        Assert.Equal("DESC", result["desc"]);
    }
}