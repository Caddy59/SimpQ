using SimpQ.Abstractions.Enums;
using SimpQ.Core.Configuration;
using SimpQ.UnitTests.Shared.Mocks.Entities;
using System.Data;

namespace SimpQ.UnitTests.Shared.Mocks.Configurations;

/// <summary>
/// Fluent configuration with multiple keyset pagination keys.
/// </summary>
public class MockEntityWithMultipleKeysetFluentConfiguration : IEntityTypeConfiguration<MockEntityFluentOnly> {
    public void Configure(EntityTypeBuilder<MockEntityFluentOnly> builder) {
        builder.Property(x => x.Id)
            .HasColumn((int)SqlDbType.Int)
            .IsKeysetPaginationKey(0);

        builder.Property(x => x.Name)
            .HasColumn((int)SqlDbType.VarChar)
            .IsKeysetPaginationKey(1);

        builder.Property(x => x.Age)
            .HasColumn((int)SqlDbType.Int);

        builder.Property(x => x.CreatedDate)
            .HasColumn((int)SqlDbType.DateTime);
    }
}
