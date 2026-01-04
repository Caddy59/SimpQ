using SimpQ.Core.Configuration;
using SimpQ.Core.Helpers;
using SimpQ.UnitTests.Shared.Mocks.Configurations;
using SimpQ.UnitTests.Shared.Mocks.Entities;

namespace SimpQ.Core.UnitTests.Helpers;

/// <summary>
/// Tests for QueryHelper with fluent configuration support.
/// </summary>
public class QueryHelperFluentTests {
    private EntityConfigurationRegistry CreateRegistry() {
        var registry = new EntityConfigurationRegistry();
        registry.Register(new MockEntityFluentOnlyConfiguration());
        return registry;
    }

    [Fact]
    public void GetColumns_ShouldReturnColumnsFromFluentConfiguration() {
        // Arrange
        var registry = CreateRegistry();

        // Act
        var result = QueryHelper.GetColumns<MockEntityFluentOnly>(registry);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        Assert.Contains(result, c => c.Name == "Id" && c.PropertyName == "Id" && c.PropertyType == typeof(int) && c.DbType == 8); // SqlDbType.Int = 8
        Assert.Contains(result, c => c.Name == "FullName" && c.PropertyName == "Name" && c.PropertyType == typeof(string) && c.DbType == 22); // SqlDbType.VarChar = 22
        Assert.Contains(result, c => c.Name == "Age" && c.PropertyName == "Age" && c.PropertyType == typeof(int?) && c.DbType == 8); // SqlDbType.Int = 8
        Assert.Contains(result, c => c.Name == "CreatedDate" && c.PropertyName == "CreatedDate" && c.PropertyType == typeof(DateTime) && c.DbType == 4); // SqlDbType.DateTime = 4
    }

    [Fact]
    public void GetColumns_ShouldReturnEmptySet_WhenNoConfigurationProvided() {
        // Act
        var result = QueryHelper.GetColumns<MockEntityFluentOnly>();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetAllowedColumnsToFilter_ShouldReturnColumnsFromFluentConfiguration() {
        // Arrange
        var registry = CreateRegistry();

        // Act
        var result = QueryHelper.GetAllowedColumnsToFilter<MockEntityFluentOnly>(registry);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Name == "Id" && c.PropertyName == "Id");
        Assert.Contains(result, c => c.Name == "FullName" && c.PropertyName == "Name");
    }

    [Fact]
    public void GetAllowedColumnsToOrder_ShouldReturnColumnsFromFluentConfiguration() {
        // Arrange
        var registry = CreateRegistry();

        // Act
        var result = QueryHelper.GetAllowedColumnsToOrder<MockEntityFluentOnly>(registry);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Name == "Id" && c.PropertyName == "Id");
        Assert.Contains(result, c => c.Name == "Age" && c.PropertyName == "Age");
    }

    [Fact]
    public void GetDefaultColumnsToOrder_ShouldReturnDefaultOrderFromFluentConfiguration() {
        // Arrange
        var registry = CreateRegistry();

        // Act
        var result = QueryHelper.GetDefaultColumnsToOrder<MockEntityFluentOnly>(registry);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, c => c.Name == "Id" && c.Priority == 0 && c.Direction == OrderDirection.Ascending);
    }

    [Fact]
    public void GetDefaultColumnsToOrder_ShouldThrowException_WhenNoDefaultOrderInFluentConfiguration() {
        // Arrange
        var registry = new EntityConfigurationRegistry();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => QueryHelper.GetDefaultColumnsToOrder<MockEntityFluentOnly>(registry));
        Assert.Equal($"No default order columns found for {nameof(MockEntityFluentOnly)}.", exception.Message);
    }

    [Fact]
    public void GetOrderedKeysetColumns_ShouldReturnKeysetColumnsFromFluentConfiguration() {
        // Arrange
        var registry = new EntityConfigurationRegistry();
        registry.Register(new MockEntityWithMultipleKeysetFluentConfiguration());

        // Act
        var result = QueryHelper.GetOrderedKeysetColumns<MockEntityFluentOnly>(registry);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Id", result.ElementAt(0).Name);
        Assert.Equal("Name", result.ElementAt(1).Name);
    }

    [Fact]
    public void GetOrderedKeysetColumns_ShouldThrowException_WhenNoKeysetColumnsInFluentConfiguration() {
        // Arrange
        var registry = new EntityConfigurationRegistry();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => QueryHelper.GetOrderedKeysetColumns<MockEntityFluentOnly>(registry));
        Assert.Equal($"No keyset pagination key columns found for {nameof(MockEntityFluentOnly)}.", exception.Message);
    }

    [Fact]
    public void GetOrderedKeysetProperties_ShouldReturnKeysetPropertiesFromFluentConfiguration() {
        // Arrange
        var registry = new EntityConfigurationRegistry();
        registry.Register(new MockEntityWithMultipleKeysetFluentConfiguration());

        // Act
        var result = QueryHelper.GetOrderedKeysetProperties<MockEntityFluentOnly>(registry);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Id", result.ElementAt(0).Name);
        Assert.Equal("Name", result.ElementAt(1).Name);
    }

    [Fact]
    public void GetOrderedKeysetProperties_ShouldThrowException_WhenNoKeysetPropertiesInFluentConfiguration() {
        // Arrange
        var registry = new EntityConfigurationRegistry();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => QueryHelper.GetOrderedKeysetProperties<MockEntityFluentOnly>(registry));
        Assert.Equal($"No keyset pagination key columns found for {nameof(MockEntityFluentOnly)}.", exception.Message);
    }

    [Fact]
    public void GetColumns_ShouldPreferAttributeOverFluentConfiguration() {
        // Arrange
        var registry = CreateRegistry();

        // Act - Using MockEntity which has both attributes and (potentially) fluent configuration
        var result = QueryHelper.GetColumns<MockEntity>(registry);

        // Assert - Should use attribute configuration
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, c => c.Name == "Id" && c.PropertyName == "Id");
        Assert.Contains(result, c => c.Name == "FullName" && c.PropertyName == "Name");
        Assert.Contains(result, c => c.Name == "Age" && c.PropertyName == "Age");
    }

    [Fact]
    public void GetAllowedColumnsToFilter_ShouldWorkWithoutRegistry() {
        // Act
        var result = QueryHelper.GetAllowedColumnsToFilter<MockEntity>();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, c => c.Name == "FullName" && c.PropertyName == "Name");
    }

    [Fact]
    public void GetAllowedColumnsToOrder_ShouldWorkWithoutRegistry() {
        // Act
        var result = QueryHelper.GetAllowedColumnsToOrder<MockEntity>();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, c => c.Name == "Age" && c.PropertyName == "Age");
    }
}
