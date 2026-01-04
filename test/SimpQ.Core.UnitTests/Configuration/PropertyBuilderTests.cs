using SimpQ.Abstractions.Enums;
using SimpQ.Core.Configuration;
using System.Data;

namespace SimpQ.Core.UnitTests.Configuration;

public class PropertyBuilderTests {
    [Fact]
    public void HasColumn_ShouldSetDbType() {
        // Arrange
        var config = new PropertyConfiguration();
        var builder = new PropertyBuilder<object, int>(config, "TestProperty");

        // Act
        builder.HasColumn((int)SqlDbType.Int);

        // Assert
        Assert.Equal(8, config.DbType); // SqlDbType.Int = 8
    }

    [Fact]
    public void HasColumn_ShouldSetColumnName() {
        // Arrange
        var config = new PropertyConfiguration();
        var builder = new PropertyBuilder<object, int>(config, "TestProperty");

        // Act
        builder.HasColumn((int)SqlDbType.Int, "CustomName");

        // Assert
        Assert.Equal("CustomName", config.ColumnName);
    }

    [Fact]
    public void HasColumn_ShouldUsePropertyName_WhenColumnNameNotProvided() {
        // Arrange
        var config = new PropertyConfiguration();
        var builder = new PropertyBuilder<object, int>(config, "TestProperty");

        // Act
        builder.HasColumn((int)SqlDbType.Int);

        // Assert
        Assert.Equal("TestProperty", config.ColumnName);
    }

    [Fact]
    public void AllowedToFilter_ShouldSetFlag() {
        // Arrange
        var config = new PropertyConfiguration();
        var builder = new PropertyBuilder<object, int>(config, "TestProperty");

        // Act
        builder.AllowedToFilter();

        // Assert
        Assert.True(config.AllowedToFilter);
    }

    [Fact]
    public void AllowedToOrder_ShouldSetFlag() {
        // Arrange
        var config = new PropertyConfiguration();
        var builder = new PropertyBuilder<object, int>(config, "TestProperty");

        // Act
        builder.AllowedToOrder();

        // Assert
        Assert.True(config.AllowedToOrder);
    }

    [Fact]
    public void IsKeysetPaginationKey_ShouldSetFlagAndPriority() {
        // Arrange
        var config = new PropertyConfiguration();
        var builder = new PropertyBuilder<object, int>(config, "TestProperty");

        // Act
        builder.IsKeysetPaginationKey(5);

        // Assert
        Assert.True(config.IsKeysetPaginationKey);
        Assert.Equal(5, config.KeysetPaginationPriority);
    }

    [Fact]
    public void IsKeysetPaginationKey_ShouldUseDefaultPriority_WhenNotProvided() {
        // Arrange
        var config = new PropertyConfiguration();
        var builder = new PropertyBuilder<object, int>(config, "TestProperty");

        // Act
        builder.IsKeysetPaginationKey();

        // Assert
        Assert.True(config.IsKeysetPaginationKey);
        Assert.Equal(0, config.KeysetPaginationPriority);
    }

    [Fact]
    public void HasDefaultOrder_ShouldSetFlagPriorityAndDirection() {
        // Arrange
        var config = new PropertyConfiguration();
        var builder = new PropertyBuilder<object, int>(config, "TestProperty");

        // Act
        builder.HasDefaultOrder(3, OrderDirection.Descending);

        // Assert
        Assert.True(config.IsDefaultOrder);
        Assert.Equal(3, config.DefaultOrderPriority);
        Assert.Equal(OrderDirection.Descending, config.DefaultOrderDirection);
    }

    [Fact]
    public void HasDefaultOrder_ShouldUseDefaults_WhenNotProvided() {
        // Arrange
        var config = new PropertyConfiguration();
        var builder = new PropertyBuilder<object, int>(config, "TestProperty");

        // Act
        builder.HasDefaultOrder();

        // Assert
        Assert.True(config.IsDefaultOrder);
        Assert.Equal(0, config.DefaultOrderPriority);
        Assert.Equal(OrderDirection.Ascending, config.DefaultOrderDirection);
    }

    [Fact]
    public void PropertyBuilder_ShouldSupportMethodChaining() {
        // Arrange
        var config = new PropertyConfiguration();
        var builder = new PropertyBuilder<object, int>(config, "TestProperty");

        // Act
        var result = builder
            .HasColumn((int)SqlDbType.Int, "CustomName")
            .AllowedToFilter()
            .AllowedToOrder()
            .IsKeysetPaginationKey(1)
            .HasDefaultOrder(2, OrderDirection.Descending);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(8, config.DbType); // SqlDbType.Int = 8
        Assert.Equal("CustomName", config.ColumnName);
        Assert.True(config.AllowedToFilter);
        Assert.True(config.AllowedToOrder);
        Assert.True(config.IsKeysetPaginationKey);
        Assert.Equal(1, config.KeysetPaginationPriority);
        Assert.True(config.IsDefaultOrder);
        Assert.Equal(2, config.DefaultOrderPriority);
        Assert.Equal(OrderDirection.Descending, config.DefaultOrderDirection);
    }
}
