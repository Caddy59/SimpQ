using SimpQ.Core.Helpers;

namespace SimpQ.Core.UnitTests.Helpers;

public class QueryHelperTests {
    [Fact]
    public void GetColumns_ShouldReturnAllColumnsWithColumnAttribute() {
        // Act
        var result = QueryHelper.GetColumns<MockEntity>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, c => c.Name == "Id" && c.PropertyName == "Id" && c.PropertyType == typeof(int) && c.DbType == 1);
        Assert.Contains(result, c => c.Name == "FullName" && c.PropertyName == "Name" && c.PropertyType == typeof(string) && c.DbType == 2);
        Assert.Contains(result, c => c.Name == "Age" && c.PropertyName == "Age" && c.PropertyType == typeof(int?) && c.DbType == 3);
    }

    [Fact]
    public void GetAllowedColumnsToFilter_ShouldReturnColumnsWithAllowedToFilterAttribute() {
        // Act
        var result = QueryHelper.GetAllowedColumnsToFilter<MockEntity>();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, c => c.Name == "FullName" && c.PropertyName == "Name");
    }

    [Fact]
    public void GetAllowedColumnsToOrder_ShouldReturnColumnsWithAllowedToOrderAttribute() {
        // Act
        var result = QueryHelper.GetAllowedColumnsToOrder<MockEntity>();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, c => c.Name == "Age" && c.PropertyName == "Age");
    }

    [Fact]
    public void GetDefaultColumnsToOrder_ShouldReturnDefaultOrderColumns() {
        // Act
        var result = QueryHelper.GetDefaultColumnsToOrder<MockEntity>();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, c => c.Name == "Id" && c.Priority == 0 && c.Direction == OrderDirection.Ascending);
    }

    [Fact]
    public void GetDefaultColumnsToOrder_ShouldThrowException_WhenNoDefaultOrderColumnExists() {
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => QueryHelper.GetDefaultColumnsToOrder<MockEntityWithoutDefaultOrder>());
        Assert.Equal($"No default order columns found for {nameof(MockEntityWithoutDefaultOrder)}.", exception.Message);
    }

    [Fact]
    public void GetDefaultColumnsToOrder_ShouldThrowException_WhenConflictingDefaultOrderColumnsExist() {
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => QueryHelper.GetDefaultColumnsToOrder<MockEntityWithConflictingDefaultOrder>());
        Assert.StartsWith($"Duplicated priorities found for {nameof(MockEntityWithConflictingDefaultOrder)}:", exception.Message);
    }

    [Fact]
    public void GetOrderedKeysetColumns_ShouldReturnOrderedKeysetColumns() {
        // Act
        var result = QueryHelper.GetOrderedKeysetColumns<MockEntityWithMultipleKeysetKey>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Id", result.ElementAt(0).Name);
        Assert.Equal("Name", result.ElementAt(1).Name);
    }

    [Fact]
    public void GetOrderedKeysetColumns_ShouldThrowException_WhenNoKeysetColumnsExist() {
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => QueryHelper.GetOrderedKeysetColumns<MockEntityWithoutKeysetKey>());
        Assert.Equal($"No keyset pagination key columns found for {nameof(MockEntityWithoutKeysetKey)}.", exception.Message);
    }

    [Fact]
    public void GetOrderedKeysetColumns_ShouldThrowException_WhenConflictingKeysetPrioritiesExist() {
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => QueryHelper.GetOrderedKeysetColumns<MockEntityWithConflictingKeysetKey>());
        Assert.StartsWith($"Duplicated keyset pagination priorities found for {nameof(MockEntityWithConflictingKeysetKey)}:", exception.Message);
    }

    [Fact]
    public void GetOrderedKeysetProperties_ShouldReturnOrderedKeysetProperties() {
        // Act
        var result = QueryHelper.GetOrderedKeysetProperties<MockEntityWithMultipleKeysetKey>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Id", result.ElementAt(0).Name);
        Assert.Equal("Name", result.ElementAt(1).Name);
    }

    [Fact]
    public void GetOrderedKeysetProperties_ShouldThrowException_WhenNoKeysetPropertiesExist() {
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => QueryHelper.GetOrderedKeysetProperties<MockEntityWithoutKeysetKey>());
        Assert.Equal($"No keyset pagination key columns found for {nameof(MockEntityWithoutKeysetKey)}.", exception.Message);
    }
}