using System.Data;

namespace SimpQ.SqlServer.UnitTests.Helpers;

public class EntityHelperTests {
    [Fact]
    public void GetEntity_ShouldMapIDataReaderToEntity() {
        // Arrange
        var id = 1;
        var fullName = "Alice";
        var age = 42;
        using var reader = GetIDataReader(id, fullName, age);
        reader.Read();

        // Act
        var result = reader.GetEntity<MockEntity>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(fullName, result.Name);
        Assert.Equal(age, result.Age);
    }
    
    [Fact]
    public void GetEntity_ShouldSkipNullValues() {
        // Arrange
        var id = 1;
        var fullName = "Alice";
        var age = (int?)null;
        using var reader = GetIDataReader(id, fullName, age);
        reader.Read();

        // Act
        var result = reader.GetEntity<MockEntity>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(fullName, result.Name);
        Assert.Equal(age, result.Age);
    }

    /*[Fact]
    public void GetEntity_ShouldHandleTypeConversion() {
        // Arrange
        var mockDataReader = new Mock<SqlDataReader>();
        mockDataReader.Setup(r => r.FieldCount).Returns(1);
        mockDataReader.Setup(r => r.GetName(0)).Returns("DateOfBirth");
        mockDataReader.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns(false);
        mockDataReader.Setup(r => r.GetValue(0)).Returns(new DateTime(1990, 1, 1));

        // Act
        var result = mockDataReader.Object.GetEntity<MockEntityWithDate>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new DateOnly(1990, 1, 1), result.DateOfBirth);
    }*/

    [Fact]
    public void GetSqlParameters_ShouldConvertParametersToSqlParameters() {
        // Arrange
        var parameters = new List<Parameter> {
            new("@p1", 1, (int)SqlDbType.Int),
            new("@p2", "Alice", (int)SqlDbType.VarChar),
            new("@p3", new DateOnly(1964, 5, 4), (int)SqlDbType.DateTime)
        };

        // Act
        var result = EntityHelper.GetSqlParameters(parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Length);
        Assert.Equal("@p1", result[0].ParameterName);
        Assert.Equal(1, result[0].Value);
        Assert.Equal(SqlDbType.Int, result[0].SqlDbType);
        Assert.Equal("@p2", result[1].ParameterName);
        Assert.Equal("Alice", result[1].Value);
        Assert.Equal(SqlDbType.VarChar, result[1].SqlDbType);
        Assert.Equal("@p3", result[2].ParameterName);
        Assert.Equal(new DateOnly(1964, 5, 4), result[2].Value);
        Assert.Equal(SqlDbType.DateTime, result[2].SqlDbType);
    }

    private static IDataReader GetIDataReader(int id, string fullName, int? age) {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("FullName", typeof(string));
        table.Columns.Add("Age", typeof(int));
        table.Rows.Add(id, fullName, age);
        return table.CreateDataReader();
    }
}