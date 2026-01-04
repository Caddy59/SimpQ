using Microsoft.Extensions.DependencyInjection;
using SimpQ.Abstractions.Reports;
using SimpQ.Core.Configuration;
using System.Reflection;

namespace SimpQ.Core.Extensions;

/// <summary>
/// Extension methods for registering fluent configuration services.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Adds entity configuration services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureAction">Optional action to configure entity types.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddEntityConfiguration(this IServiceCollection services, Action<EntityConfigurationBuilder>? configureAction = null) {
        services.AddSingleton<EntityConfigurationRegistry>();
        
        if (configureAction is not null) {
            var builder = new EntityConfigurationBuilder(services);
            configureAction(builder);
        }

        return services;
    }

    /// <summary>
    /// Registers entity configurations from the specified assembly.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembly">The assembly to scan for entity configurations.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddEntityConfigurationsFromAssembly(this IServiceCollection services, Assembly assembly) {
        var registry = services.BuildServiceProvider().GetService<EntityConfigurationRegistry>()
            ?? throw new InvalidOperationException("EntityConfigurationRegistry not registered. Call AddEntityConfiguration first.");

        registry.RegisterFromAssembly(assembly);
        return services;
    }

    /// <summary>
    /// Registers entity configurations from the calling assembly.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddEntityConfigurationsFromAssembly(this IServiceCollection services) =>
        services.AddEntityConfigurationsFromAssembly(Assembly.GetCallingAssembly());
}

/// <summary>
/// Builder for configuring entity types.
/// </summary>
public class EntityConfigurationBuilder {
    private readonly IServiceCollection _services;

    internal EntityConfigurationBuilder(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// Applies configuration for an entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to configure.</typeparam>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The builder for method chaining.</returns>
    public EntityConfigurationBuilder ApplyConfiguration<TEntity>(IEntityTypeConfiguration<TEntity> configuration) 
        where TEntity : class, IReportEntity {
        _services.AddSingleton(configuration);
        _services.AddSingleton<Func<EntityConfigurationRegistry, EntityConfigurationRegistry>>(provider => registry => {
            registry.Register(configuration);
            return registry;
        });
        return this;
    }

    /// <summary>
    /// Applies configurations from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for configurations.</param>
    /// <returns>The builder for method chaining.</returns>
    public EntityConfigurationBuilder ApplyConfigurationsFromAssembly(Assembly assembly) {
        _services.AddSingleton<Func<EntityConfigurationRegistry, EntityConfigurationRegistry>>(provider => registry => {
            registry.RegisterFromAssembly(assembly);
            return registry;
        });
        return this;
    }
}