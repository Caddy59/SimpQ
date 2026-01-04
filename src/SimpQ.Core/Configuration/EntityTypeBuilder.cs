using SimpQ.Abstractions.Reports;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpQ.Core.Configuration;

/// <summary>
/// Fluent builder for configuring an entity type.
/// </summary>
/// <typeparam name="TEntity">The entity type to configure.</typeparam>
public class EntityTypeBuilder<TEntity> where TEntity : class, IReportEntity {
    private readonly Dictionary<string, PropertyConfiguration> _properties = [];

    /// <summary>
    /// Configures a property of the entity using a fluent API.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyExpression">Expression that selects the property to configure.</param>
    /// <returns>A property builder for configuring the selected property.</returns>
    public PropertyBuilder<TEntity, TProperty> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression) {
        var memberExpression = propertyExpression.Body as MemberExpression
            ?? throw new ArgumentException("Property expression must be a simple property access.", nameof(propertyExpression));

        var propertyInfo = memberExpression.Member as PropertyInfo
            ?? throw new ArgumentException("Expression must select a property.", nameof(propertyExpression));

        var propertyName = propertyInfo.Name;

        if (!_properties.TryGetValue(propertyName, out PropertyConfiguration? value)) {
            value = new PropertyConfiguration();
            _properties[propertyName] = value;
        }

        return new PropertyBuilder<TEntity, TProperty>(value, propertyName);
    }

    /// <summary>
    /// Gets the configuration for all properties.
    /// </summary>
    /// <returns>A read-only dictionary of property configurations.</returns>
    internal IReadOnlyDictionary<string, PropertyConfiguration> GetPropertyConfigurations() => _properties.AsReadOnly();
}