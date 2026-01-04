using SimpQ.Abstractions.Enums;
using SimpQ.Core.Configuration;
using SimpQ.UnitTests.Shared.Mocks.Entities;
using System.Data;

namespace SimpQ.Core.UnitTests.Configuration;

public class EntityTypeBuilderTests {
    [Fact]
    public void Property_ShouldReturnPropertyBuilder() {
        // Arrange
        var builder = new EntityTypeBuilder<MockEntityFluentOnly>();

        // Act
        var propertyBuilder = builder.Property(x => x.Id);

        // Assert
        Assert.NotNull(propertyBuilder);
    }

    [Fact]
    public void Property_ShouldThrowArgumentException_WhenExpressionIsNotProperty() {
        // Arrange
        var builder = new EntityTypeBuilder<MockEntityFluentOnly>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.Property(x => x.ToString()));
    }

    [Fact]
    public void Property_ShouldConfigureMultipleProperties() {
        // Arrange
        var builder = new EntityTypeBuilder<MockEntityFluentOnly>();

        // Act
        builder.Property(x => x.Id).HasColumn((int)SqlDbType.Int);
        builder.Property(x => x.Name).HasColumn((int)SqlDbType.VarChar);

        // Assert - Verify through EntityConfigurationRegistry
        var registry = new ReportEntityConfigurationRegistry();
        var testConfig = new TestEntityTypeConfiguration(builder);
        registry.Register(testConfig);
        var result = registry.GetConfiguration(typeof(MockEntityFluentOnly));

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.True(result.ContainsKey("Id"));
        Assert.True(result.ContainsKey("Name"));
    }

    [Fact]
    public void Property_ShouldReuseExistingConfiguration_WhenCalledMultipleTimes() {
        // Arrange
        var builder = new EntityTypeBuilder<MockEntityFluentOnly>();

        // Act
        builder.Property(x => x.Id).HasColumn((int)SqlDbType.Int);
        builder.Property(x => x.Id).AllowedToFilter();

        // Assert - Verify through EntityConfigurationRegistry
        var registry = new ReportEntityConfigurationRegistry();
        var testConfig = new TestEntityTypeConfiguration(builder);
        registry.Register(testConfig);
        var result = registry.GetConfiguration(typeof(MockEntityFluentOnly));

        var idConfig = result!["Id"];
        Assert.Equal(8, idConfig.DbType); // SqlDbType.Int = 8
        Assert.True(idConfig.AllowedToFilter);
    }

    // Helper class to test EntityTypeBuilder
    private class TestEntityTypeConfiguration : IReportEntityTypeConfiguration<MockEntityFluentOnly> {
        private readonly EntityTypeBuilder<MockEntityFluentOnly> _existingBuilder;

        public TestEntityTypeConfiguration(EntityTypeBuilder<MockEntityFluentOnly> existingBuilder) {
            _existingBuilder = existingBuilder;
        }

        public void Configure(EntityTypeBuilder<MockEntityFluentOnly> builder) {
            // This will be called by the registry, but we ignore the new builder
            // and use reflection to copy the properties from the existing builder
            var field = typeof(EntityTypeBuilder<MockEntityFluentOnly>)
                .GetField("_properties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var properties = field!.GetValue(_existingBuilder) as Dictionary<string, PropertyConfiguration>;
            var newProperties = field.GetValue(builder) as Dictionary<string, PropertyConfiguration>;
            foreach (var prop in properties!) {
                newProperties![prop.Key] = prop.Value;
            }
        }
    }
}
