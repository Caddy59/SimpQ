using SimpQ.Abstractions.Attributes.Entities;
using SimpQ.Abstractions.Models.Internal;
using SimpQ.Abstractions.Reports;
using SimpQ.Core.Configuration;
using System.Collections.Frozen;
using System.Reflection;

namespace SimpQ.Core.Helpers;

/// <summary>
/// Provides reflection-based helpers for extracting column and ordering metadata from classes implementing <see cref="IReportEntity"/>.
/// Supports both attribute-based and fluent configuration.
/// </summary>
public static class QueryHelper {
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
            
            if (attributeConfig is not null) {
                columns.Add(new Column(attributeConfig.Name, attributeConfig.PropertyName, property.PropertyType, attributeConfig.DbType));
                continue;
            }

            if (fluentConfig?.TryGetValue(property.Name, out var propConfig) == true && propConfig.DbType != 0) {
                var columnName = !string.IsNullOrWhiteSpace(propConfig.ColumnName) ? propConfig.ColumnName : property.Name;
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

            if (columnAttr is not null && defaultOrderAttr is not null) {
                defaultOrders.Add((columnAttr.Name, defaultOrderAttr.Priority, defaultOrderAttr.OrderDirection));
                continue;
            }

            if (fluentConfig?.TryGetValue(property.Name, out var propConfig) == true && 
                propConfig.IsDefaultOrder && 
                propConfig.DbType != 0) {
                var columnName = !string.IsNullOrWhiteSpace(propConfig.ColumnName) ? propConfig.ColumnName : property.Name;
                defaultOrders.Add((columnName, propConfig.DefaultOrderPriority, propConfig.DefaultOrderDirection));
            }
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

            if (columnAttr is not null && keysetAttr is not null) {
                var column = new Column(columnAttr.Name, columnAttr.PropertyName, property.PropertyType, columnAttr.DbType);
                keysetColumns.Add((column, keysetAttr.Priority));
                continue;
            }

            if (fluentConfig?.TryGetValue(property.Name, out var propConfig) == true && 
                propConfig.IsKeysetPaginationKey && 
                propConfig.DbType != 0) {
                var columnName = !string.IsNullOrWhiteSpace(propConfig.ColumnName) ? propConfig.ColumnName : property.Name;
                var column = new Column(columnName, property.Name, property.PropertyType, propConfig.DbType);
                keysetColumns.Add((column, propConfig.KeysetPaginationPriority));
            }
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

            if (columnAttr is not null && keysetAttr is not null) {
                keysetProperties.Add((property, keysetAttr.Priority));
                continue;
            }

            if (fluentConfig?.TryGetValue(property.Name, out var propConfig) == true && 
                propConfig.IsKeysetPaginationKey && 
                propConfig.DbType != 0) {
                keysetProperties.Add((property, propConfig.KeysetPaginationPriority));
            }
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
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <typeparam name="TAttribute">The attribute type that marks a column as eligible (e.g., AllowedToFilterAttribute).</typeparam>
    /// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
    /// <param name="fluentPredicate">Predicate to check if a property configuration meets the criteria.</param>
    /// <returns>A frozen set of matching columns.</returns>
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

            if (columnAttr is not null && allowedAttr is not null) {
                columns.Add(new Column(columnAttr.Name, columnAttr.PropertyName, property.PropertyType, columnAttr.DbType));
                continue;
            }

            if (fluentConfig?.TryGetValue(property.Name, out var propConfig) == true && 
                fluentPredicate(propConfig) && 
                propConfig.DbType != 0) {
                var columnName = !string.IsNullOrWhiteSpace(propConfig.ColumnName) ? propConfig.ColumnName : property.Name;
                columns.Add(new Column(columnName, property.Name, property.PropertyType, propConfig.DbType));
            }
        }

        return columns.ToFrozenSet();
    }
}