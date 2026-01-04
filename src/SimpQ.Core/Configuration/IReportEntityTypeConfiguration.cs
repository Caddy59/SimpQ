using SimpQ.Abstractions.Reports;

namespace SimpQ.Core.Configuration;

/// <summary>
/// Allows configuration of report entity type metadata using a fluent API.
/// </summary>
/// <typeparam name="TEntity">The entity type to configure.</typeparam>
public interface IReportEntityTypeConfiguration<TEntity> where TEntity : class, IReportEntity {
    /// <summary>
    /// Configures the entity type using a fluent API builder.
    /// </summary>
    /// <param name="builder">The entity type builder to configure with.</param>
    void Configure(EntityTypeBuilder<TEntity> builder);
}