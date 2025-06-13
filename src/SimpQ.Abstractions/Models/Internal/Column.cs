namespace SimpQ.Abstractions.Models.Internal;

/// <summary>
/// Represents metadata about a column used for mapping between a SQL result set and a .NET entity property.
/// </summary>
/// <param name="Name">The name of the column as it appears in the SQL result set.</param>
/// <param name="PropertyName">The name of the corresponding property in the entity class.</param>
/// <param name="PropertyType">The .NET type of the property.</param>
/// <param name="DbType">The integer identifier representing the database type (e.g., SQL Server type code).</param>
public record Column(string Name, string PropertyName, Type PropertyType, int DbType);