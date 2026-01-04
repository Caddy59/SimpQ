using SimpQ.Abstractions.Enums;
using SimpQ.Core.Configuration;
using SimpQ.UnitTests.Shared.Mocks.Entities;
using System.Data;

namespace SimpQ.UnitTests.Shared.Mocks.Configurations;

/// <summary>
/// Fluent configuration for MockEntityFluentOnly.
/// </summary>
public class MockEntityFluentOnlyConfiguration : IReportEntityTypeConfiguration<MockEntityFluentOnly> {
    public void Configure(EntityTypeBuilder<MockEntityFluentOnly> builder) {
        builder.Property(x => x.Id)
            .HasColumn((int)SqlDbType.Int)
            .AllowedToFilter()
            .AllowedToOrder()
            .IsKeysetPaginationKey(0)
            .HasDefaultOrder(0, OrderDirection.Ascending);

        builder.Property(x => x.Name)
            .HasColumn((int)SqlDbType.VarChar, "FullName")
            .AllowedToFilter();

        builder.Property(x => x.Age)
            .HasColumn((int)SqlDbType.Int)
            .AllowedToOrder();

        builder.Property(x => x.CreatedDate)
            .HasColumn((int)SqlDbType.DateTime);
    }
}
