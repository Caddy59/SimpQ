using SimpQ.Abstractions.Exceptions;
using SimpQ.Abstractions.Models.Requests;

namespace SimpQ.SqlServer.UnitTests.Queries.ClauseBuilders;

public class SelectClauseBuilderTests {
    [Fact]
    public void Build_ShouldUseAllColumns_WhenSelectIsNull() {
        // Arrange
        var rawQuery = "SELECT * FROM Person";
        // Act
        var sql = SelectClauseBuilder.Build<MockEntity>(select: null, rawQuery);

        // Assert
        Assert.Equal($"SELECT\r\n[Id]\r\n,[FullName]\r\n,[Age]\r\nFROM ({rawQuery}) \"result\"", sql);
    }

    [Fact]
    public void Build_ShouldIncludeOnlySelectedColumns_WhenSelectIsProvided() {
        // Arrange
        var rawQuery = "SELECT * FROM Person";

        var select = new List<Select> {
            new() { Field = "Age" }
        };

        // Act
        var sql = SelectClauseBuilder.Build<MockEntity>(select, rawQuery);

        // Assert
        Assert.Equal($"SELECT\r\n[Age]\r\nFROM ({rawQuery}) \"result\"", sql);
    }

    [Fact]
    public void Build_ShouldThrowInvalidColumException_WhenSelectContainsInvalidField() {
        // Arrange
        var rawQuery = "SELECT * FROM Person";

        var select = new List<Select> {
            new() { Field = "InvalidField" }
        };

        // Act & Assert
        Assert.Throws<InvalidColumException>(() => SelectClauseBuilder.Build<MockEntity>(select, rawQuery));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Build_ShouldThrowArgumentException_WhenRawQueryIsWhiteSpace(string? rawQuery) {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => SelectClauseBuilder.Build<MockEntity>(select: null, rawQuery!));
    }

    [Fact]
    public void Build_ShouldThrowArgumentNullException_WhenRawQueryIsNulL() {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => SelectClauseBuilder.Build<MockEntity>(select: null, null!));
    }
}