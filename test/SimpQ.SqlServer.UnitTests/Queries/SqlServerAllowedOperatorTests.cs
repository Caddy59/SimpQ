using SimpQ.SqlServer.Queries;

namespace SimpQ.SqlServer.UnitTests.Queries;

public class SqlServerAllowedOperatorTests {
    [Fact]
    public void ComparisonOperators_ShouldContainExpectedMappings() {
        // Act
        var comparisonOperators = SqlServerAllowedOperator.ComparisonOperators;

        // Assert
        Assert.NotNull(comparisonOperators);
        Assert.NotEmpty(comparisonOperators);
        Assert.Equal("IS NULL", comparisonOperators["is_null"]);
        Assert.Equal("IS NOT NULL", comparisonOperators["is_not_null"]);
        Assert.Equal("=", comparisonOperators["equals"]);
        Assert.Equal("<>", comparisonOperators["not_equals"]);
        Assert.Equal(">=", comparisonOperators["greater_equals"]);
        Assert.Equal("<=", comparisonOperators["less_equals"]);
        Assert.Equal(">", comparisonOperators["greater"]);
        Assert.Equal("<", comparisonOperators["less"]);
        Assert.Equal("LIKE", comparisonOperators["contains"]);
        Assert.Equal("NOT LIKE", comparisonOperators["not_contains"]);
        Assert.Equal("LIKE", comparisonOperators["starts_with"]);
        Assert.Equal("LIKE", comparisonOperators["ends_with"]);
        Assert.Equal("IN", comparisonOperators["in"]);
        Assert.Equal("NOT IN", comparisonOperators["not_in"]);
        Assert.Equal("BETWEEN", comparisonOperators["between"]);
        Assert.Equal("NOT BETWEEN", comparisonOperators["not_between"]);
    }

    [Fact]
    public void LogicalOperators_ShouldContainExpectedMappings() {
        // Act
        var logicalOperators = SqlServerAllowedOperator.LogicalOperators;

        // Assert
        Assert.NotNull(logicalOperators);
        Assert.NotEmpty(logicalOperators);
        Assert.Equal("AND", logicalOperators["and"]);
        Assert.Equal("OR", logicalOperators["or"]);
    }

    [Fact]
    public void OrderingOperators_ShouldContainExpectedMappings() {
        // Act
        var orderingOperators = SqlServerAllowedOperator.OrderingOperators;

        // Assert
        Assert.NotNull(orderingOperators);
        Assert.NotEmpty(orderingOperators);
        Assert.Equal("ASC", orderingOperators["asc"]);
        Assert.Equal("DESC", orderingOperators["desc"]);
    }
}