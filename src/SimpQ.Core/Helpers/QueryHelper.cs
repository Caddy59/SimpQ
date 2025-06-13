using SimpQ.Abstractions.Attributes.Entities;
using SimpQ.Abstractions.Models.Internal;
using SimpQ.Abstractions.Reports;
using System.Collections.Frozen;
using System.Reflection;

namespace SimpQ.Core.Helpers;

/// <summary>
/// Provides reflection-based helpers for extracting column and ordering metadata from classes implementing <see cref="IReportEntity"/>.
/// </summary>
public static class QueryHelper {
    /// <summary>
    /// Retrieves all properties of the entity decorated with <see cref="ColumnAttribute"/>, 
    /// and maps them to a frozen set of <see cref="Column"/> metadata.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <returns>A frozen set of columns representing mapped properties.</returns>
    public static FrozenSet<Column> GetColumns<TEntity>() where TEntity : IReportEntity {
        return typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ColumnAttribute>() is not null)
            .Select(p => {
                var attribute = p.GetCustomAttribute<ColumnAttribute>()!;
                return new Column(attribute.Name, attribute.PropertyName, p.PropertyType, attribute.DbType);
            })
            .ToFrozenSet();
    }

    /// <summary>
    /// Retrieves the properties of the entity that are allowed to be used as filters,
    /// based on the presence of the <see cref="AllowedToFilterAttribute"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <returns>A frozen set of filterable columns.</returns>
    public static FrozenSet<Column> GetAllowedColumnsToFilter<TEntity>() where TEntity : IReportEntity => GetAllowedFields<TEntity, AllowedToFilterAttribute>();

    /// <summary>
    /// Retrieves the properties of the entity that are allowed to be used in ordering,
    /// based on the presence of the <see cref="AllowedToOrderAttribute"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <returns>A frozen set of orderable columns.</returns>
    public static FrozenSet<Column> GetAllowedColumnsToOrder<TEntity>() where TEntity : IReportEntity => GetAllowedFields<TEntity, AllowedToOrderAttribute>();

    /// <summary>
    /// Retrieves the default ordering configuration for the entity by inspecting 
    /// properties decorated with both <see cref="ColumnAttribute"/> and <see cref="DefaultOrderAttribute"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <returns>A read-only collection of <see cref="ColumnOrder"/> entries.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no default ordering is found or if duplicate priorities exist.</exception>
    public static IReadOnlyCollection<ColumnOrder> GetDefaultColumnsToOrder<TEntity>() where TEntity : IReportEntity {
        var annotated = typeof(TEntity)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ColumnAttribute>() is not null &&
                        p.GetCustomAttribute<DefaultOrderAttribute>() is not null)
            .Select(p => new {
                Property = p,
                ColumnAttribute = p.GetCustomAttribute<ColumnAttribute>()!,
                DefaultOrderAttribute = p.GetCustomAttribute<DefaultOrderAttribute>()!
            })
            .ToArray();

        if (annotated.Length == 0)
            throw new InvalidOperationException($"No default order columns found for {typeof(TEntity).Name}.");

        var duplicateGroups = annotated.GroupBy(x => x.DefaultOrderAttribute.Priority)
            .Where(g => g.Count() > 1)
            .ToList();

        if (duplicateGroups.Count > 0) {
            var conflicts = duplicateGroups.Select(g => $"Priority {g.Key}: {string.Join(", ", g.Select(x => x.Property.Name))}")
                .ToList();
            throw new InvalidOperationException($"Duplicated priorities found for {typeof(TEntity).Name}:{Environment.NewLine}{string.Join(Environment.NewLine, conflicts)}");
        }

        var columns = annotated
            .OrderBy(x => x.DefaultOrderAttribute.Priority)
            .Select(x => new ColumnOrder(x.ColumnAttribute.Name, x.DefaultOrderAttribute.Priority, x.DefaultOrderAttribute.OrderDirection))
            .ToArray();

        return columns.AsReadOnly();
    }

    /// <summary>
    /// Retrieves the columns marked with <see cref="KeysetPaginationKeyAttribute"/>, 
    /// ordered by the attribute's priority.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <returns>A read-only ordered collection of keyset pagination columns.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no pagination keys are found or if duplicate priorities exist.</exception>
    public static IReadOnlyCollection<Column> GetOrderedKeysetColumns<TEntity>() where TEntity : IReportEntity {
        var annotated = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ColumnAttribute>() is not null &&
                        p.GetCustomAttribute<KeysetPaginationKeyAttribute>() is not null)
            .Select(p => new {
                Property = p,
                ColumnAttribute = p.GetCustomAttribute<ColumnAttribute>()!,
                KeysetAttribute = p.GetCustomAttribute<KeysetPaginationKeyAttribute>()!
            }).ToArray();

        if (annotated.Length == 0)
            throw new InvalidOperationException($"No keyset pagination key columns found for {typeof(TEntity).Name}.");

        var duplicateGroups = annotated.GroupBy(x => x.KeysetAttribute.Priority)
            .Where(g => g.Count() > 1)
            .ToList();

        if (duplicateGroups.Count > 0) {
            var conflicts = duplicateGroups.Select(g => $"Priority {g.Key}: {string.Join(", ", g.Select(x => x.Property.Name))}")
                .ToList();
            throw new InvalidOperationException($"Duplicated keyset pagination priorities found for {typeof(TEntity).Name}:{Environment.NewLine}{string.Join(Environment.NewLine, conflicts)}");
        }

        var columns = annotated
            .OrderBy(x => x.KeysetAttribute.Priority)
            .Select(x => new Column(x.ColumnAttribute.Name, x.ColumnAttribute.PropertyName, x.Property.PropertyType, x.ColumnAttribute.DbType))
            .ToArray();

        return columns.AsReadOnly();
    }

    /// <summary>
    /// Retrieves the properties marked with <see cref="KeysetPaginationKeyAttribute"/>, 
    /// ordered by the attribute's priority.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <returns>A read-only list of keyset pagination properties.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no pagination keys are found.</exception>
    public static IReadOnlyCollection<PropertyInfo> GetOrderedKeysetProperties<TEntity>() where TEntity : IReportEntity {
        var properties = typeof(TEntity)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ColumnAttribute>() is not null &&
                        p.GetCustomAttribute<KeysetPaginationKeyAttribute>() is not null)
            .Select(p => new {
                Property = p,
                p.GetCustomAttribute<KeysetPaginationKeyAttribute>()!.Priority
            })
            .OrderBy(x => x.Priority)
            .Select(x => x.Property)
            .ToArray();

        if (properties.Length == 0)
            throw new InvalidOperationException($"No keyset pagination key columns found for {typeof(TEntity).Name}.");

        return properties.AsReadOnly();
    }

    /// <summary>
    /// Retrieves the set of columns from <typeparamref name="TEntity"/> that are annotated with both 
    /// <see cref="ColumnAttribute"/> and a specified attribute type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to inspect.</typeparam>
    /// <typeparam name="TAttribute">The attribute that marks a column as eligible (e.g., <see cref="AllowedToFilterAttribute"/>).</typeparam>
    /// <returns>A frozen set of matching columns.</returns>
    private static FrozenSet<Column> GetAllowedFields<TEntity, TAttribute>() where TEntity : IReportEntity where TAttribute : Attribute {
        return typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<TAttribute>() is not null && p.GetCustomAttribute<ColumnAttribute>() is not null)
            .Select(p => {
                var attribute = p.GetCustomAttribute<ColumnAttribute>()!;
                return new Column(attribute.Name, attribute.PropertyName, p.PropertyType, attribute.DbType);
            })
            .ToFrozenSet();
    }
}