namespace SimpQ.Abstractions.Models.Internal;

/// <summary>
/// Represents a SQL parameter used in raw queries, including its name, value, and database type.
/// </summary>
/// <param name="Name">The name of the parameter (e.g., "@p0").</param>
/// <param name="Value">The value to be assigned to the parameter. Can be <c>null</c>.</param>
/// <param name="DbType">The integer identifier representing the database type (e.g., SQL Server type code).</param>
public record Parameter(string Name, object? Value, int DbType);