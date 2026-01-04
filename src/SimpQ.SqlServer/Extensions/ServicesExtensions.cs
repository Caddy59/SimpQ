using SimpQ.SqlServer.Queries;
using SimpQ.SqlServer.Reports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpQ.SqlServer.Queries.ClauseBuilders;
using SimpQ.SqlServer.Queries.OperatorHandlers;
using SimpQ.Core.Options;
using SimpQ.Core.Configuration;

namespace SimpQ.SqlServer.Extensions;

/// <summary>
/// Extension methods for registering SimpQ services with SQL Server support.
/// </summary>
public static class ServicesExtensions {
    /// <summary>
    /// Registers the required SimpQ services for working with SQL Server-based query execution.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="connectionString">The SQL Server connection string used for raw query execution.</param>
    /// <param name="configureOptions">
    /// Optional delegate to configure <see cref="SimpQOptions"/>, such as setting the maximum filter nesting level.
    /// </param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddSimpQSqlServer(this IServiceCollection services, string connectionString, Action<SimpQOptions>? configureOptions = null) {
        if (configureOptions is not null)
            services.Configure(configureOptions);

        services.AddSingleton<EntityConfigurationRegistry>();

        services.AddSingleton<ValidOperator>()
            .AddSingleton<SimpQOperator>()
            .AddSingleton<SqlServerQueryOperator>()
            .AddSingleton<WhereClauseBuilder>()
            .AddSingleton<OrderClauseBuilder>()
            .AddSingleton<WhereOperatorHandlerResolver>()
            .AddWhereOperatorHandlers();

        services.AddScoped<IQueryDefinitionFactory, SqlServerQueryDefinitionFactory>()
            .AddScoped<IReportQueryRaw, SqlServerReportQueryRaw>(s => new SqlServerReportQueryRaw(
                s.GetRequiredService<ILogger<SqlServerReportQueryRaw>>(), 
                connectionString, 
                s.GetRequiredService<IQueryDefinitionFactory>(), 
                s.GetService<EntityConfigurationRegistry>()));

        return services;
    }

    /// <summary>
    /// Registers all supported <see cref="IWhereOperatorHandler"/> implementations in a specific order of precedence.
    /// These handlers are responsible for translating individual filter conditions to SQL fragments.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance for chaining.</returns>
    private static IServiceCollection AddWhereOperatorHandlers(this IServiceCollection services) {
        // NOTE: The order of precedence is important here, always order by the most used first.
        var orderedHandlers = new Type[] {
            typeof(DefaultOperatorHandler),
            typeof(InOperatorHandler),
            typeof(LikeOperatorHandler),
            typeof(NullCheckOperatorHandler),
            typeof(BetweenOperatorHandler)
        };

        foreach (var type in orderedHandlers)
            services.AddSingleton(typeof(IWhereOperatorHandler), type);

        return services;
    }
}