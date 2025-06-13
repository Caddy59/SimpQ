using SimpQ.Abstractions.Attributes.Entities;
using SimpQ.Abstractions.Enums;

namespace SimpQ.Abstractions.UnitTests.Attributes;

public class ColumnAttributeTests {
    [Fact]
    public void Constructor_ShouldSetDefaultValues_WhenRequiredParametersProvided() {
        // Act
        var attribute = new ColumnAttribute(1);

        // Assert
        Assert.Equal(1, attribute.DbType);
        Assert.NotNull(attribute.PropertyName);
        Assert.Equal(attribute.PropertyName, attribute.Name);
    }

    [Fact]
    public void Constructor_ShouldSetName_WhenNameIsProvided() {
        // Arrange
        const string customName = "CustomColumnName";

        // Act
        var attribute = new ColumnAttribute(1, name: customName);

        // Assert
        Assert.Equal(1, attribute.DbType);
        Assert.Equal(customName, attribute.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Constructor_ShouldFallbackToPropertyName_WhenNameIsNullOrWhitespace(string? name) {
        // Act
        var attribute = new ColumnAttribute(1, name: name);

        // Assert
        Assert.Equal(attribute.PropertyName, attribute.Name);
    }
}