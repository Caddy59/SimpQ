using SimpQ.Core.Configuration;
using SimpQ.UnitTests.Shared.Mocks.Configurations;
using SimpQ.UnitTests.Shared.Mocks.Entities;

namespace SimpQ.Core.UnitTests.Configuration;

public class EntityConfigurationRegistryTests {
    [Fact]
    public void Register_ShouldStoreConfiguration() {
        // Arrange
        var registry = new EntityConfigurationRegistry();
        var configuration = new MockEntityFluentOnlyConfiguration();

        // Act
        registry.Register(configuration);

        // Assert
        Assert.True(registry.HasConfiguration(typeof(MockEntityFluentOnly)));
    }

    [Fact]
    public void GetConfiguration_ShouldReturnStoredConfiguration() {
        // Arrange
        var registry = new EntityConfigurationRegistry();
        var configuration = new MockEntityFluentOnlyConfiguration();
        registry.Register(configuration);

        // Act
        var result = registry.GetConfiguration(typeof(MockEntityFluentOnly));

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ContainsKey("Id"));
        Assert.True(result.ContainsKey("Name"));
        Assert.True(result.ContainsKey("Age"));
        Assert.True(result.ContainsKey("CreatedDate"));
    }

    [Fact]
    public void GetConfiguration_ShouldReturnNull_WhenConfigurationNotRegistered() {
        // Arrange
        var registry = new EntityConfigurationRegistry();

        // Act
        var result = registry.GetConfiguration(typeof(MockEntity));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void HasConfiguration_ShouldReturnFalse_WhenConfigurationNotRegistered() {
        // Arrange
        var registry = new EntityConfigurationRegistry();

        // Act
        var result = registry.HasConfiguration(typeof(MockEntity));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RegisterFromAssembly_ShouldRegisterAllConfigurations() {
        // Arrange
        var registry = new EntityConfigurationRegistry();
        var assembly = typeof(MockEntityFluentOnlyConfiguration).Assembly;

        // Act
        registry.RegisterFromAssembly(assembly);

        // Assert
        Assert.True(registry.HasConfiguration(typeof(MockEntityFluentOnly)));
    }

    [Fact]
    public void GetConfiguration_ShouldReturnCorrectPropertyConfiguration() {
        // Arrange
        var registry = new EntityConfigurationRegistry();
        var configuration = new MockEntityFluentOnlyConfiguration();
        registry.Register(configuration);

        // Act
        var result = registry.GetConfiguration(typeof(MockEntityFluentOnly));

        // Assert
        Assert.NotNull(result);

        // Verify Id configuration
        var idConfig = result["Id"];
        Assert.Equal(8, idConfig.DbType); // SqlDbType.Int = 8
        Assert.True(idConfig.AllowedToFilter);
        Assert.True(idConfig.AllowedToOrder);
        Assert.True(idConfig.IsKeysetPaginationKey);
        Assert.Equal(0, idConfig.KeysetPaginationPriority);
        Assert.True(idConfig.IsDefaultOrder);
        Assert.Equal(0, idConfig.DefaultOrderPriority);

        // Verify Name configuration
        var nameConfig = result["Name"];
        Assert.Equal(22, nameConfig.DbType); // SqlDbType.VarChar = 22
        Assert.Equal("FullName", nameConfig.ColumnName);
        Assert.True(nameConfig.AllowedToFilter);
        Assert.False(nameConfig.AllowedToOrder);
    }
}
