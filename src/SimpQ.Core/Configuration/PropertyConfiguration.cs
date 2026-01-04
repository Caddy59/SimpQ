using SimpQ.Abstractions.Enums;

namespace SimpQ.Core.Configuration;

/// <summary>
/// Contains metadata configuration for a specific property.
/// </summary>
public class PropertyConfiguration {
    /// <summary>
    /// Gets or sets the database type identifier.
    /// </summary>
    public int DbType { get; set; }

    /// <summary>
    /// Gets or sets the column name in the database.
    /// </summary>
    public string? ColumnName { get; set; }

    /// <summary>
    /// Gets or sets whether this property is allowed to be filtered.
    /// </summary>
    public bool AllowedToFilter { get; set; }

    /// <summary>
    /// Gets or sets whether this property is allowed to be ordered.
    /// </summary>
    public bool AllowedToOrder { get; set; }

    /// <summary>
    /// Gets or sets whether this property is a keyset pagination key.
    /// </summary>
    public bool IsKeysetPaginationKey { get; set; }

    /// <summary>
    /// Gets or sets the priority for keyset pagination (lower values are evaluated first).
    /// </summary>
    public int KeysetPaginationPriority { get; set; }

    /// <summary>
    /// Gets or sets whether this property has a default order.
    /// </summary>
    public bool IsDefaultOrder { get; set; }

    /// <summary>
    /// Gets or sets the priority for default ordering (lower values have higher precedence).
    /// </summary>
    public int DefaultOrderPriority { get; set; }

    /// <summary>
    /// Gets or sets the default order direction.
    /// </summary>
    public OrderDirection DefaultOrderDirection { get; set; } = OrderDirection.Ascending;
}