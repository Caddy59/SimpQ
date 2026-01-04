using SimpQ.Abstractions.Reports;

namespace SimpQ.DemoApi.ReportEntities;

/// <summary>
/// Invoice entity configured entirely using fluent configuration (no attributes).
/// This demonstrates how to use the fluent API without any attribute decoration.
/// </summary>
public class InvoiceFluentOnly : IReportEntity {
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string CashierName { get; set; } = default!;
    public string ProductName { get; set; } = default!;
    public decimal Price { get; set; }
}