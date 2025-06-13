using System.Runtime.CompilerServices;

namespace SimpQ.Abstractions.Attributes.Entities;

/// <summary>
/// Specifies metadata for mapping a property to a column in the underlying data source.
/// </summary>
/// <param name="dbType">The integer identifier representing the database type (e.g., SQL Server type code).</param>
/// <param name="name">The name of the column in the database (optional). If it is not provided, the propertyName will be used.</param>
/// <param name="propertyName">The name of the property in the class. Automatically filled if not provided.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ColumnAttribute(int dbType, string? name = null, [CallerMemberName] string? propertyName = null) : Attribute {
    /// <summary>
    /// Gets the database type identifier associated with the column.
    /// </summary>
    public int DbType { get; private init; } = dbType;
    /// <summary>
    /// Gets the name of the property this attribute is applied to.
    /// </summary>
    public string PropertyName { get; private init; } = propertyName!;
    /// <summary>
    /// Gets the name of the database column.
    /// If not explicitly provided, defaults to the property name.
    /// </summary>
    public string Name { get; private init; } = string.IsNullOrWhiteSpace(name) ? propertyName! : name;
}