using SimpQ.Core.Serialization;
using System.Text.Json;

namespace SimpQ.Core.UnitTests.Serialization;

public class SimpQFilterJsonConverterTests {
    private readonly JsonSerializerOptions _options;

    public SimpQFilterJsonConverterTests() {
        _options = new JsonSerializerOptions {
            Converters = { new SimpQFilterJsonConverter() },
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public void Read_ShouldDeserializeFilterGroup_WhenJsonContainsConditions() {
        // Arrange
        var json = @"{
                ""conditions"": [
                    { ""field"": ""Name"", ""operator"": ""eq"", ""value"": ""John"" }
                ],
                ""logic"": ""and""
            }";

        // Act
        var result = JsonSerializer.Deserialize<IFilter>(json, _options);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FilterGroup>(result);

        var filterGroup = (FilterGroup)result;
        Assert.Equal("and", filterGroup.Logic);
        Assert.Single(filterGroup.Conditions);
        Assert.IsType<FilterCondition>(filterGroup.Conditions.First());

        var condition = (FilterCondition)filterGroup.Conditions.First();
        Assert.Equal("Name", condition.Field);
        Assert.Equal("eq", condition.Operator);
        Assert.Equal("John", condition.Value.ToString());
    }

    [Fact]
    public void Read_ShouldDeserializeFilterCondition_WhenJsonDoesNotContainConditions() {
        // Arrange
        var json = @"{
                ""field"": ""Age"",
                ""operator"": ""gt"",
                ""value"": 30
            }";

        // Act
        var result = JsonSerializer.Deserialize<IFilter>(json, _options);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FilterCondition>(result);

        var condition = (FilterCondition)result;
        Assert.Equal("Age", condition.Field);
        Assert.Equal("gt", condition.Operator);
        Assert.Equal(30, Convert.ToInt32(condition.Value.ToString()));
    }

    [Fact]
    public void Write_ShouldSerializeFilterGroupCorrectly() {
        // Arrange
        var filterGroup = new FilterGroup {
            Logic = "or",
            Conditions = [
                new FilterCondition { Field = "Name", Operator = "eq", Value = "Alice" },
                new FilterCondition { Field = "Age", Operator = "lt", Value = 25 }
            ]
        };

        // Act
        var json = JsonSerializer.Serialize(filterGroup, _options);

        // Assert
        Assert.Contains(@"""Logic"":""or""", json);
        Assert.Contains(@"""Conditions"":[", json);
        Assert.Contains(@"""Field"":""Name""", json);
        Assert.Contains(@"""Operator"":""eq""", json);
        Assert.Contains(@"""Value"":""Alice""", json);
        Assert.Contains(@"""Field"":""Age""", json);
        Assert.Contains(@"""Operator"":""lt""", json);
        Assert.Contains(@"""Value"":25", json);
    }

    [Fact]
    public void Write_ShouldSerializeFilterConditionCorrectly() {
        // Arrange
        var filterCondition = new FilterCondition {
            Field = "Salary",
            Operator = "gt",
            Value = 50000
        };

        // Act
        var json = JsonSerializer.Serialize(filterCondition, _options);

        // Assert
        Assert.Contains(@"""Field"":""Salary""", json);
        Assert.Contains(@"""Operator"":""gt""", json);
        Assert.Contains(@"""Value"":50000", json);
    }
}