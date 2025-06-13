using SimpQ.Core.Queries;

namespace SimpQ.SqlServer.UnitTests.Helpers;

public class ValidOperatorTests {
    private readonly SimpQOperator _simpQOperator = new();
    private readonly ValidOperator _validOperator;

    public ValidOperatorTests() {
        _validOperator = new ValidOperator(_simpQOperator);
    }

    [Theory]
    [InlineData("is_null")]
    [InlineData("is_not_null")]
    [InlineData("equals")]
    [InlineData("not_equals")]
    [InlineData("contains")]
    [InlineData("not_contains")]
    [InlineData("starts_with")]
    [InlineData("ends_with")]
    [InlineData("greater_equals")]
    [InlineData("greater")]
    [InlineData("less_equals")]
    [InlineData("less")]
    [InlineData("in")]
    [InlineData("not_in")]
    public void IsOperatorValidForText_ShouldReturnTrue_ForTextTypes(string @operator) {
        // Arrange
        var types = new[] {
            typeof(string),
            typeof(char)
        };

        foreach (var type in types) {
            // Act
            var result = _validOperator.IsOperatorValidForType(type, @operator);

            // Assert
            Assert.True(result);
        }
    }

    [Theory]
    [InlineData("is_null")]
    [InlineData("is_not_null")]
    [InlineData("equals")]
    [InlineData("not_equals")]
    public void IsOperatorValidForBool_ShouldReturnTrue_ForBooleanTypes(string @operator) {
        // Act
        var result = _validOperator.IsOperatorValidForType(typeof(bool), @operator);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("is_null")]
    [InlineData("is_not_null")]
    [InlineData("equals")]
    [InlineData("not_equals")]
    [InlineData("greater_equals")]
    [InlineData("greater")]
    [InlineData("less_equals")]
    [InlineData("less")]
    [InlineData("in")]
    [InlineData("not_in")]
    public void IsOperatorValidForGuid_ShouldReturnTrue_ForGuidTypes(string @operator) {
        // Act
        var result = _validOperator.IsOperatorValidForType(typeof(Guid), @operator);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("is_null")]
    [InlineData("is_not_null")]
    [InlineData("equals")]
    [InlineData("not_equals")]
    [InlineData("greater_equals")]
    [InlineData("greater")]
    [InlineData("less_equals")]
    [InlineData("less")]
    [InlineData("in")]
    [InlineData("not_in")]
    [InlineData("between")]
    [InlineData("not_between")]
    public void IsOperatorValidForDateTime_ShouldReturnTrue_ForDateTypes(string @operator) {
        // Arrange
        var types = new[] {
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(DateOnly),
            typeof(TimeOnly)
        };

        foreach (var type in types) {
            // Act
            var result = _validOperator.IsOperatorValidForType(type, @operator);

            // Assert
            Assert.True(result);
        }
    }

    [Theory]
    [InlineData("is_null")]
    [InlineData("is_not_null")]
    [InlineData("equals")]
    [InlineData("not_equals")]
    [InlineData("greater_equals")]
    [InlineData("greater")]
    [InlineData("less_equals")]
    [InlineData("less")]
    [InlineData("in")]
    [InlineData("not_in")]
    [InlineData("between")]
    [InlineData("not_between")]
    public void IsOperatorValidForNumber_ShouldReturnTrue_ForNumericTypes(string @operator) {
        // Arrange
        var types = new[] { 
            typeof(Byte),
            typeof(SByte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Int128),
            typeof(UInt128),
            typeof(Single),
            typeof(Double),
            typeof(Decimal)
        };

        foreach (var type in types) {
            // Act
            var result = _validOperator.IsOperatorValidForType(type, @operator);

            // Assert
            Assert.True(result);
        }
    }

    [Fact]
    public void IsOperatorValidForType_ShouldReturnFalse_ForInvalidOperatorAndType() {
        // Act
        var result = _validOperator.IsOperatorValidForType(typeof(string), _simpQOperator.Between);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsOperatorValidForType_ShouldReturnFalse_ForUnsupportedType() {
        // Act
        var result = _validOperator.IsOperatorValidForType(typeof(object), _simpQOperator.Equals);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(["is_null", "is_not_null", "equals", "not_equals", "contains", "not_contains", "starts_with", "ends_with", "greater_equals", "greater", "less_equals", "less", "in", "not_in"])]
    public void GetOperatorsForType_ShouldReturnValidOperators_ForTextType(params string[] operators) {
        // Act
        var result = _validOperator.GetOperatorsForType(typeof(string));

        // Assert
        Assert.NotNull(result);
        foreach (var @operator in operators)
            Assert.Contains(@operator, result);
    }

    [Theory]
    [InlineData(["is_null", "is_not_null", "equals", "not_equals"])]
    public void GetOperatorsForType_ShouldReturnValidOperators_ForBooleanType(params string[] operators) {
        // Act
        var result = _validOperator.GetOperatorsForType(typeof(bool));

        // Assert
        Assert.NotNull(result);
        foreach (var @operator in operators)
            Assert.Contains(@operator, result);
    }

    [Theory]
    [InlineData(["is_null", "is_not_null", "equals", "not_equals", "greater_equals", "greater", "less_equals", "less", "in", "not_in"])]
    public void GetOperatorsForType_ShouldReturnValidOperators_ForGuidType(params string[] operators) {
        // Act
        var result = _validOperator.GetOperatorsForType(typeof(Guid));

        // Assert
        Assert.NotNull(result);
        foreach (var @operator in operators)
            Assert.Contains(@operator, result);
    }

    [Theory]
    [InlineData(["is_null", "is_not_null", "equals", "not_equals", "greater_equals", "greater", "less_equals", "less", "in", "not_in", "between", "not_between"])]
    public void GetOperatorsForType_ShouldReturnValidOperators_ForDateType(params string[] operators) {
        // Act
        var result = _validOperator.GetOperatorsForType(typeof(DateTime));

        // Assert
        Assert.NotNull(result);
        foreach (var @operator in operators)
            Assert.Contains(@operator, result);
    }

    [Theory]
    [InlineData(["is_null", "is_not_null", "equals", "not_equals", "greater_equals", "greater", "less_equals", "less", "in", "not_in", "between", "not_between"])]
    public void GetOperatorsForType_ShouldReturnValidOperators_ForNumericType(params string[] operators) {
        // Act
        var result = _validOperator.GetOperatorsForType(typeof(int));

        // Assert
        Assert.NotNull(result);
        foreach (var @operator in operators)
            Assert.Contains(@operator, result);
    }

    [Fact]
    public void GetOperatorsForType_ShouldReturnEmptyArray_ForUnsupportedType() {
        // Act
        var result = _validOperator.GetOperatorsForType(typeof(object));

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}