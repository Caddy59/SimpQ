using SimpQ.Abstractions.Enums;

namespace SimpQ.Core.Configuration;

/// <summary>
/// Fluent builder for configuring a specific property.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TProperty">The property type.</typeparam>
public class PropertyBuilder<TEntity, TProperty>(PropertyConfiguration configuration, string propertyName) {

    /// <summary>
    /// Configures the database type and optional column name for the property.
    /// </summary>
    /// <param name="dbType">The database type identifier (e.g., SQL Server type code).</param>
    /// <param name="columnName">The optional column name. If not provided, the property name will be used.</param>
    /// <returns>The property builder for method chaining.</returns>
    public PropertyBuilder<TEntity, TProperty> HasColumn(int dbType, string? columnName = null) {
        configuration.DbType = dbType;
        configuration.ColumnName = columnName ?? propertyName;
        return this;
    }

    /// <summary>
    /// Marks the property as allowed to be used in filtering.
    /// </summary>
    /// <returns>The property builder for method chaining.</returns>
    public PropertyBuilder<TEntity, TProperty> AllowedToFilter() {
        configuration.AllowedToFilter = true;
        return this;
    }

    /// <summary>
    /// Marks the property as allowed to be used in ordering.
    /// </summary>
    /// <returns>The property builder for method chaining.</returns>
    public PropertyBuilder<TEntity, TProperty> AllowedToOrder() {
        configuration.AllowedToOrder = true;
        return this;
    }

    /// <summary>
    /// Configures the property as a keyset pagination key.
    /// </summary>
    /// <param name="priority">The priority for ordering keyset keys (lower values are evaluated first). Default is 0.</param>
    /// <returns>The property builder for method chaining.</returns>
    public PropertyBuilder<TEntity, TProperty> IsKeysetPaginationKey(int priority = 0) {
        configuration.IsKeysetPaginationKey = true;
        configuration.KeysetPaginationPriority = priority;
        return this;
    }

    /// <summary>
    /// Configures the property as having a default order.
    /// </summary>
    /// <param name="priority">The priority of this order relative to others (lower values have higher precedence). Default is 0.</param>
    /// <param name="direction">The direction of the order (ascending or descending). Default is ascending.</param>
    /// <returns>The property builder for method chaining.</returns>
    public PropertyBuilder<TEntity, TProperty> HasDefaultOrder(int priority = 0, OrderDirection direction = OrderDirection.Ascending) {
        configuration.IsDefaultOrder = true;
        configuration.DefaultOrderPriority = priority;
        configuration.DefaultOrderDirection = direction;
        return this;
    }
}