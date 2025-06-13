using SimpQ.Abstractions.Models.Requests;
using SimpQ.Core.Queries;
using SimpQ.SqlServer.Queries;

namespace SimpQ.SqlServer.UnitTests.Queries;
/*
public class SqlServerQueryBuilderTests {
    private readonly Mock<ValidOperator> _mockValidOperator = new();
    private readonly Mock<SimpQOperator> _mockSimpQOperator = new();
    private readonly SqlServerQueryDefinitionFactory _queryBuilder;

    public SqlServerQueryBuilderTests() {
        _queryBuilder = new SqlServerQueryDefinitionFactory(_mockValidOperator.Object, _mockSimpQOperator.Object);
    }

    [Fact]
    public void BuildQuery_ShouldReturnValidQuery() {
        // Arrange
        var baseQuery = "SELECT * FROM Invoices";
        var preQuery = "WITH CTE AS (SELECT * FROM Invoices)";
        var request = new QueryParams {
            Select = null,
            Filters = null,
            Order = null
        };

        // Act
        var result = _queryBuilder.BuildQueryDefinition<MockEntity>(baseQuery, preQuery, request);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("SELECT", result.RowsCommand);
        Assert.Contains("FROM (SELECT * FROM Invoices) \"result\"", result.RowsCommand);
    }

    [Fact]
    public void BuildPagedQuery_ShouldReturnValidPagedQuery() {
        // Arrange
        var baseQuery = "SELECT * FROM Invoices";
        var preQuery = "WITH CTE AS (SELECT * FROM Invoices)";
        var request = new OffsetPagedQueryParams {
            Select = null,
            Filters = null,
            Order = null,
            Page = 1,
            PageSize = 10
        };
        var limit = 100;

        // Act
        var result = _queryBuilder.BuildOffsetPagedQueryDefinition<MockEntity>(baseQuery, preQuery, request, limit);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;", result.RowsCommand);
    }

    [Fact]
    public void BuildPagedQuery_ShouldThrowException_WhenPageSizeExceedsLimit() {
        // Arrange
        var baseQuery = "SELECT * FROM Invoices";
        var preQuery = "WITH CTE AS (SELECT * FROM Invoices)";
        var request = new OffsetPagedQueryParams {
            Select = null,
            Filters = null,
            Order = null,
            Page = 1,
            PageSize = 200
        };
        var limit = 100;

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _queryBuilder.BuildOffsetPagedQueryDefinition<MockEntity>(baseQuery, preQuery, request, limit));

        Assert.Equal("Page size cannot exceed 100. Current page size: 200.", exception.Message);
    }

    [Fact]
    public void BuildQuery_ShouldIncludeWhereClause_WhenFiltersAreProvided() {
        // Arrange
        var baseQuery = "SELECT * FROM Invoices";
        var preQuery = string.Empty;
        var request = new QueryParams {
            Select = null,
            Filters = new FilterGroup {
                Logic = "AND",
                Conditions = [
                    new FilterCondition { Field = "Name", Operator = "equals", Value = "Test" }
                ]
            },
            Order = null
        };

        // Act
        var result = _queryBuilder.BuildQueryDefinition<MockEntity>(baseQuery, preQuery, request);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("WHERE", result.RowsCommand);
        Assert.Contains("[Name] = @p0", result.RowsCommand);
    }

    [Fact]
    public void BuildQuery_ShouldIncludeOrderByClause_WhenOrderIsProvided() {
        // Arrange
        var baseQuery = "SELECT * FROM Invoices";
        var preQuery = string.Empty;
        var request = new QueryParams {
            Select = null,
            Filters = null,
            Order = [
                new() { Field = "Name", Direction = "ASC" }
            ]
        };

        // Act
        var result = _queryBuilder.BuildQueryDefinition<MockEntity>(baseQuery, preQuery, request);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("ORDER BY", result.RowsCommand);
        Assert.Contains("[Name] ASC", result.RowsCommand);
    }
}*/