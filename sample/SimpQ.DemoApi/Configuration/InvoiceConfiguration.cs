using SimpQ.Core.Configuration;
using SimpQ.DemoApi.ReportEntities;
using System.Data;

namespace SimpQ.DemoApi.Configuration;

/// <summary>
/// Fluent configuration for the Invoice entity as an alternative to attribute-based configuration.
/// This demonstrates how to configure entity metadata without using attributes.
/// </summary>
public class InvoiceConfiguration : IReportEntityTypeConfiguration<Invoice> {
    public void Configure(EntityTypeBuilder<Invoice> builder) {
        builder.Property(x => x.Id)
            .HasColumn((int)SqlDbType.Int, "TransactionId")
            .AllowedToFilter()
            .AllowedToOrder()
            .IsKeysetPaginationKey()
            .HasDefaultOrder();

        builder.Property(x => x.Date)
            .HasColumn((int)SqlDbType.DateTime)
            .AllowedToFilter()
            .IsKeysetPaginationKey(1);

        builder.Property(x => x.CashierName)
            .HasColumn((int)SqlDbType.VarChar)
            .AllowedToFilter();

        builder.Property(x => x.ProductName)
            .HasColumn((int)SqlDbType.VarChar)
            .AllowedToFilter()
            .AllowedToOrder();

        builder.Property(x => x.Price)
            .HasColumn((int)SqlDbType.Decimal)
            .AllowedToOrder();
    }
}