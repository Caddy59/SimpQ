using SimpQ.Abstractions.Attributes.Entities;
using SimpQ.Abstractions.Models.Internal;
using SimpQ.Abstractions.Reports;
using SimpQ.Core.Configuration;
using System.Collections.Frozen;
using System.Reflection;

namespace SimpQ.Core.Helpers;

/// <summary>
/// Provides extension methods for QueryHelper that support fluent configuration alongside attributes.
/// </summary>
public static class QueryHelperExtensions {
    /// <summary>
    /// Retrieves all properties of the entity with both attribute-based and fluent configuration support.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
    /// <returns>A frozen set of columns representing mapped properties.</returns>
    public static FrozenSet<Column> GetColumns<TEntity>(EntityConfigurationRegistry? configurationRegistry = null) 
        where TEntity : IReportEntity {
        var properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var columns = new List<Column>();
        var fluentConfig = configurationRegistry?.GetConfiguration(typeof(TEntity));

        foreach (var property in properties) {
            var attributeConfig = property.GetCustomAttribute<ColumnAttribute>();
            var hasFluentConfig = fluentConfig?.TryGetValue(property.Name, out var propConfig) == true && propConfig.DbType != 0;

            if (attributeConfig is not null)
                columns.Add(new Column(attributeConfig.Name, attributeConfig.PropertyName, property.PropertyType, attributeConfig.DbType));
            else if (hasFluentConfig) {
                var columnName = !string.IsNullOrEmpty(propConfig!.ColumnName) ? propConfig.ColumnName : property.Name;
                columns.Add(new Column(columnName, property.Name, property.PropertyType, propConfig.DbType));
            }
        }

        return columns.ToFrozenSet();
    }

    /// <summary>
    /// Retrieves properties allowed to be used as filters with fluent configuration support.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
    /// <returns>A frozen set of filterable columns.</returns>
    public static FrozenSet<Column> GetAllowedColumnsToFilter<TEntity>(EntityConfigurationRegistry? configurationRegistry = null) 
        where TEntity : IReportEntity =>
        GetAllowedFields<TEntity, AllowedToFilterAttribute>(configurationRegistry, config => config.AllowedToFilter);

    /// <summary>
    /// Retrieves properties allowed to be used in ordering with fluent configuration support.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
    /// <returns>A frozen set of orderable columns.</returns>
    public static FrozenSet<Column> GetAllowedColumnsToOrder<TEntity>(EntityConfigurationRegistry? configurationRegistry = null) 
        where TEntity : IReportEntity =>
        GetAllowedFields<TEntity, AllowedToOrderAttribute>(configurationRegistry, config => config.AllowedToOrder);

    /// <summary>
    /// Retrieves default ordering configuration with fluent configuration support.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
    /// <returns>A read-only collection of ColumnOrder entries.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no default ordering is found or if duplicate priorities exist.</exception>
    public static IReadOnlyCollection<ColumnOrder> GetDefaultColumnsToOrder<TEntity>(EntityConfigurationRegistry? configurationRegistry = null) 
        where TEntity : IReportEntity {
        var properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var defaultOrders = new List<(string ColumnName, int Priority, Abstractions.Enums.OrderDirection Direction)>();
        var fluentConfig = configurationRegistry?.GetConfiguration(typeof(TEntity));

        foreach (var property in properties) {
            var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
            var defaultOrderAttr = property.GetCustomAttribute<DefaultOrderAttribute>();
            var hasFluentConfig = fluentConfig?.TryGetValue(property.Name, out var propConfig) == true;

            var columnName = (string?)null;
            var hasDefaultOrder = false;
            var priority = 0;
            var direction = Abstractions.Enums.OrderDirection.Ascending;

            if (columnAttr is not null && defaultOrderAttr is not null) {
                columnName = columnAttr.Name;
                hasDefaultOrder = true;
                priority = defaultOrderAttr.Priority;
                direction = defaultOrderAttr.OrderDirection;
            }
            else if (hasFluentConfig && propConfig!.IsDefaultOrder && propConfig.DbType != 0) {
                columnName = !string.IsNullOrWhiteSpace(propConfig.ColumnName) ? propConfig.ColumnName : property.Name;
                hasDefaultOrder = true;
                priority = propConfig.DefaultOrderPriority;
                direction = propConfig.DefaultOrderDirection;
            }

            if (hasDefaultOrder && columnName is not null)
                defaultOrders.Add((columnName, priority, direction));
        }

        if (defaultOrders.Count == 0)
            throw new InvalidOperationException($"No default order columns found for {typeof(TEntity).Name}.");

        var duplicateGroups = defaultOrders.GroupBy(x => x.Priority)
            .Where(g => g.Count() > 1)
            .ToList();

        if (duplicateGroups.Count > 0) {
            var conflicts = duplicateGroups.Select(g => $"Priority {g.Key}: {string.Join(", ", g.Select(x => x.ColumnName))}")
                .ToList();
            throw new InvalidOperationException($"Duplicated priorities found for {typeof(TEntity).Name}:{Environment.NewLine}{string.Join(Environment.NewLine, conflicts)}");
        }

        var columns = defaultOrders
            .OrderBy(x => x.Priority)
            .Select(x => new ColumnOrder(x.ColumnName, x.Priority, x.Direction))
            .ToArray();

        return columns.AsReadOnly();
    }

    /// <summary>
    /// Retrieves keyset pagination columns with fluent configuration support.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
    /// <returns>A read-only ordered collection of keyset pagination columns.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no pagination keys are found or if duplicate priorities exist.</exception>
    public static IReadOnlyCollection<Column> GetOrderedKeysetColumns<TEntity>(EntityConfigurationRegistry? configurationRegistry = null) 
        where TEntity : IReportEntity {
        var properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var keysetColumns = new List<(Column Column, int Priority)>();
        var fluentConfig = configurationRegistry?.GetConfiguration(typeof(TEntity));

        foreach (var property in properties) {
            var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
            var keysetAttr = property.GetCustomAttribute<KeysetPaginationKeyAttribute>();
            var hasFluentConfig = fluentConfig?.TryGetValue(property.Name, out var propConfig) == true;

            Column? column = null;
            bool isKeysetKey = false;
            int priority = 0;

            if (columnAttr is not null && keysetAttr is not null) {
                column = new Column(columnAttr.Name, columnAttr.PropertyName, property.PropertyType, columnAttr.DbType);
                isKeysetKey = true;
                priority = keysetAttr.Priority;
            }
            else if (hasFluentConfig && propConfig!.IsKeysetPaginationKey && propConfig.DbType != 0) {
                var columnName = !string.IsNullOrWhiteSpace(propConfig.ColumnName) ? propConfig.ColumnName : property.Name;
                column = new Column(columnName, property.Name, property.PropertyType, propConfig.DbType);
                isKeysetKey = true;
                priority = propConfig.KeysetPaginationPriority;
            }

            if (isKeysetKey && column is not null)
                keysetColumns.Add((column, priority));
        }

        if (keysetColumns.Count == 0)
            throw new InvalidOperationException($"No keyset pagination key columns found for {typeof(TEntity).Name}.");

        var duplicateGroups = keysetColumns.GroupBy(x => x.Priority)
            .Where(g => g.Count() > 1)
            .ToList();

        if (duplicateGroups.Count > 0) {
            var conflicts = duplicateGroups.Select(g => $"Priority {g.Key}: {string.Join(", ", g.Select(x => x.Column.PropertyName))}")
                .ToList();
            throw new InvalidOperationException($"Duplicated keyset pagination priorities found for {typeof(TEntity).Name}:{Environment.NewLine}{string.Join(Environment.NewLine, conflicts)}");
        }

        var columns = keysetColumns
            .OrderBy(x => x.Priority)
            .Select(x => x.Column)
            .ToArray();

        return columns.AsReadOnly();
    }

    /// <summary>
    /// Retrieves keyset pagination properties with fluent configuration support.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
    /// <returns>A read-only list of keyset pagination properties.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no pagination keys are found.</exception>
    public static IReadOnlyCollection<PropertyInfo> GetOrderedKeysetProperties<TEntity>(EntityConfigurationRegistry? configurationRegistry = null) 
        where TEntity : IReportEntity {
        var properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var keysetProperties = new List<(PropertyInfo Property, int Priority)>();
        var fluentConfig = configurationRegistry?.GetConfiguration(typeof(TEntity));

        foreach (var property in properties) {
            var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
            var keysetAttr = property.GetCustomAttribute<KeysetPaginationKeyAttribute>();
            var hasFluentConfig = fluentConfig?.TryGetValue(property.Name, out var propConfig) == true;

            bool isKeysetKey = false;
            int priority = 0;

            if (columnAttr is not null && keysetAttr is not null) {
                isKeysetKey = true;
                priority = keysetAttr.Priority;
            }
            else if (hasFluentConfig && propConfig!.IsKeysetPaginationKey && propConfig.DbType != 0) {
                isKeysetKey = true;
                priority = propConfig.KeysetPaginationPriority;
            }

            if (isKeysetKey)
                keysetProperties.Add((property, priority));
        }

        if (keysetProperties.Count == 0)
            throw new InvalidOperationException($"No keyset pagination key columns found for {typeof(TEntity).Name}.");

        var properties_result = keysetProperties
            .OrderBy(x => x.Priority)
            .Select(x => x.Property)
            .ToArray();

        return properties_result.AsReadOnly();
    }

    /// <summary>
    /// Generic method to retrieve allowed fields with fluent configuration support.
    /// </summary>
    private static FrozenSet<Column> GetAllowedFields<TEntity, TAttribute>(
        EntityConfigurationRegistry? configurationRegistry,
        Func<PropertyConfiguration, bool> fluentPredicate) 
        where TEntity : IReportEntity 
        where TAttribute : Attribute {
        var properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var columns = new List<Column>();
        var fluentConfig = configurationRegistry?.GetConfiguration(typeof(TEntity));

        foreach (var property in properties) {
            var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
            var allowedAttr = property.GetCustomAttribute<TAttribute>();
            var hasFluentConfig = fluentConfig?.TryGetValue(property.Name, out var propConfig) == true;

            var column = (Column?)null;
            var isAllowed = false;

            if (columnAttr is not null && allowedAttr is not null) {
                column = new Column(columnAttr.Name, columnAttr.PropertyName, property.PropertyType, columnAttr.DbType);
                isAllowed = true;
            }
            else if (hasFluentConfig && fluentPredicate(propConfig!) && propConfig!.DbType != 0) {
                var columnName = !string.IsNullOrWhiteSpace(propConfig.ColumnName) ? propConfig.ColumnName : property.Name;
                column = new Column(columnName, property.Name, property.PropertyType, propConfig.DbType);
                isAllowed = true;
            }

            if (isAllowed && column !   = null)
                columns.Add(column);
        }

        return columns.ToFrozenSet();
    }
}