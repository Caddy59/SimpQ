using SimpQ.Abstractions.Exceptions;
using SimpQ.Abstractions.Models.Requests;
using SimpQ.Core.Helpers;
using SimpQ.SqlServer.Queries;

namespace SimpQ.SqlServer.UnitTests.Queries.ClauseBuilders;

public class OrderClauseBuilderTests {
    private readonly SqlServerQueryOperator _sqlQueryOperator = new();
    private OrderClauseBuilder CreateBuilder() => new(_sqlQueryOperator);
    [Fact]
    public void Build_ShouldReturnValidOrderClause_WhenValidOrderProvided() {
        // Arrange
        var builder = CreateBuilder();

        var order = new List<Order> {
            new() { Field = "Age", Direction = "asc" }
        };

        // Act
        var result = builder.Build<MockEntity>(order, keysetFilter: null);

        // Assert
        Assert.Equal("ORDER BY\r\n[Age] ASC", result);
    }

    [Fact]
    public void Build_ShouldReturnDefaultOrderClause_WhenOrderIsNull() {
        // Arrange
        var builder = CreateBuilder();

        // Act
        var result = builder.Build<MockEntityWithMultipleDefaultOrder>(null, keysetFilter: null);

        // Assert
        Assert.Equal("ORDER BY\r\n[Id] ASC\r\n,[Name] DESC", result);
    }

    /*[Fact]
    public void Build_ShouldReturnEmptyString_WhenOrderIsNullAndKeysetPagination() {
        // Arrange
        var builder = CreateBuilder();

        // Act
        var result = builder.Build<MockEntityWithMultipleKeysetKey>(null, new KeysetFilter(IsDescending: false));

        // Assert
        Assert.Equal("ORDER BY\r\n[Id] ASC,\r\n[Age] ASC", result);
    }*/

    [Fact]
    public void Build_ShouldThrowInvalidColumException_WhenFieldNotAllowed() {
        // Arrange
        var builder = CreateBuilder();
        var order = new List<Order> { 
            new() { Field = "Name", Direction = "asc" } 
        };

        // Act & Assert
        Assert.Throws<InvalidColumException>(() => builder.Build<MockEntity>(order, keysetFilter: null));
    }

    [Fact]
    public void Build_ShouldThrowInvalidOrderDirectionException_WhenDirectionIsInvalid() {
        // Arrange
        var builder = CreateBuilder();
        var order = new List<Order> { 
            new() { Field = "Age", Direction = "invalid" }
        };

        // Act & Assert
        Assert.Throws<InvalidOrderDirectionException>(() => builder.Build<MockEntity>(order, keysetFilter: null));
    }

    /*[Fact]
    public void Build_ShouldThrowInvalidOperationException_WhenKeysetColumnUsedInOrder() {
        var builder = CreateBuilder();
        var order = new List<Order> { new() { Field = "Id", Direction = "asc" } };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            builder.Build<MockEntityWithMultipleKeysetKey>(order, new KeysetFilter(IsDescending: false)));

        Assert.Contains("keyset column(s)", ex.Message);
    }*/

    /*[Fact]
    public void Build_ShouldReturnKeysetOrderClause_WhenKeysetPaginationAndNoOrder() {
        var builder = CreateBuilder();

        var result = builder.Build<MockEntityWithMultipleKeysetKey>(null, new KeysetFilter(IsDescending: true));

        Assert.StartsWith("ORDER BY", result);
        Assert.Contains("[Id] DESC", result);
        Assert.Contains("[Age] DESC", result);
    }*/

    /*[Fact]
    public void Build_ShouldCombineOrderAndKeysetClauses_WhenBothProvided() {
        var builder = CreateBuilder();
        var order = new List<Order> { new() { Field = "Age", Direction = "asc" } };

        var result = builder.Build<MockEntityWithMultipleKeysetKey>(order, new KeysetFilter(IsDescending: false));

        Assert.StartsWith("ORDER BY", result);
        Assert.Contains("[Age] ASC", result);
        Assert.Contains("[Id] ASC", result);
    }

    [Fact]
    public void Build_ShouldReturnEmptyString_WhenOrderAndKeysetAreNull_AndNoDefaultOrder() {
        var builder = CreateBuilder();

        var result = builder.Build<MockEntityWithoutKeysetKey>(null, null);

        Assert.Equal(string.Empty, result);
    }

    /*[Fact]
    public void Build_ShouldThrowInvalidOperationException_WhenKeysetColumnUsedInOrder() {
        // Arrange
        var builder = CreateBuilder();

        var order = new List<Order> {
            new() { Field = "Age", Direction = "asc" }
        };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build<MockEntityWithMultipleKeysetKey>(order, new KeysetFilter(IsDescending: false)));
        Assert.Contains("keyset column(s)", ex.Message);
    }*/

    [Fact]
    public void Build_ShouldThrowInvalidColumnException_WhenFieldNotAllowed() {
        var builder = CreateBuilder();

        var order = new List<Order> {
            new() { Field = "Name", Direction = "asc" }
        };

        Assert.Throws<InvalidColumException>(() => builder.Build<MockEntity>(order, keysetFilter: null));
    }
}