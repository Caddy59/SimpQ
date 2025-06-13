using SimpQ.Core.Contexts;

namespace SimpQ.Core.UnitTests.Contexts;

public class ParameterContextTests {
    [Fact]
    public void Add_ShouldAddParameterToList() {
        // Arrange
        var context = new ParameterContext();
        var value = "TestValue";
        var dbType = 1;

        // Act
        var parameterName = context.Add(value, dbType);

        // Assert
        Assert.NotNull(parameterName);
        Assert.Equal("@p0", parameterName);
        Assert.Single(context.Parameters);

        var parameter = context.Parameters.Single();
        Assert.Equal(parameterName, parameter.Name);
        Assert.Equal(value, parameter.Value);
        Assert.Equal(dbType, parameter.DbType);
    }

    [Fact]
    public void Add_ShouldIncrementParameterIndex() {
        // Arrange
        var context = new ParameterContext();

        // Act
        var firstParameterName = context.Add("Value1", 1);
        var secondParameterName = context.Add("Value2", 2);

        // Assert
        Assert.Equal("@p0", firstParameterName);
        Assert.Equal("@p1", secondParameterName);
        Assert.Equal(2, context.Parameters.Count);
    }

    [Fact]
    public void Parameters_ShouldReturnReadOnlyCollection() {
        // Arrange
        var context = new ParameterContext();

        // Act
        var parameters = context.Parameters;

        // Assert
        Assert.IsType<IReadOnlyCollection<Parameter>>(parameters, exactMatch: false);
    }
}