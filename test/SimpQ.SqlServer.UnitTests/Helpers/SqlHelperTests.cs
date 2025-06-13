namespace SimpQ.SqlServer.UnitTests.Helpers;

public class SqlHelperTests {
    [Theory]
    [InlineData("ColumnName", "[ColumnName]")]
    [InlineData("MyColumn", "[MyColumn]")]
    [InlineData("123Column", "[123Column]")]
    [InlineData("Column With Spaces", "[Column With Spaces]")]
    [InlineData("Column-Name", "[Column-Name]")]
    public void EscapeColumnName_ShouldWrapColumnNameInBrackets(string input, string expected) {
        // Act
        var result = input.EscapeColumnName();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EscapeColumnName_ShouldHandleNullInput() {
        // Arrange
        var columnName = (string?)null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => columnName!.EscapeColumnName());
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    public void EscapeColumnName_ShouldHandleWhiteSpaceInput(string columnName) {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => columnName.EscapeColumnName());
    }
}