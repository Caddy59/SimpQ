using SimpQ.Abstractions.Reports;
using System.Collections.Concurrent;
using System.Reflection;

namespace SimpQ.Core.Configuration;

/// <summary>
/// Registry for storing and retrieving entity configurations.
/// </summary>
public class EntityConfigurationRegistry {
    private readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, PropertyConfiguration>> _configurations = new();

    /// <summary>
    /// Registers an entity configuration.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to register.</typeparam>
    /// <param name="configuration">The configuration instance.</param>
    public void Register<TEntity>(IEntityTypeConfiguration<TEntity> configuration) where TEntity : class, IReportEntity {
        var builder = new EntityTypeBuilder<TEntity>();
        configuration.Configure(builder);
        _configurations[typeof(TEntity)] = builder.GetPropertyConfigurations();
    }

    /// <summary>
    /// Registers entity configurations from an assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for configurations.</param>
    public void RegisterFromAssembly(Assembly assembly) {
        var configurationTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.GetInterfaces().Any(i => 
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
            .ToList();

        foreach (var configurationType in configurationTypes) {
            var interfaceType = configurationType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));

            var entityType = interfaceType.GetGenericArguments()[0];
            var configuration = Activator.CreateInstance(configurationType);

            var registerMethod = GetType().GetMethod(nameof(Register))!.MakeGenericMethod(entityType);
            registerMethod.Invoke(this, [configuration]);
        }
    }

    /// <summary>
    /// Gets the configuration for a specific entity type.
    /// </summary>
    /// <param name="entityType">The entity type to get configuration for.</param>
    /// <returns>The property configurations for the entity, or null if not configured.</returns>
    public IReadOnlyDictionary<string, PropertyConfiguration>? GetConfiguration(Type entityType) {
        _configurations.TryGetValue(entityType, out var configuration);
        return configuration;
    }

    /// <summary>
    /// Checks if an entity type has a fluent configuration.
    /// </summary>
    /// <param name="entityType">The entity type to check.</param>
    /// <returns>True if the entity type has a fluent configuration, false otherwise.</returns>
    public bool HasConfiguration(Type entityType) => _configurations.ContainsKey(entityType);
}