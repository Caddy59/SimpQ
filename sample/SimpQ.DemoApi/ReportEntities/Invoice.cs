using SimpQ.Abstractions.Attributes.Entities;
using SimpQ.Abstractions.Reports;
using System.Data;

namespace SimpQ.DemoApi.ReportEntities;

public class Invoice : IReportEntity {
    [Column((int)SqlDbType.Int, name: "TransactionId")]
    [KeysetPaginationKey]
    [AllowedToFilter]
    [AllowedToOrder]
    public int Id { get; set; }
    [Column((int)SqlDbType.DateTime)]
    [KeysetPaginationKey(1)]
    [AllowedToFilter]
    public DateTime Date { get; set; }
    [Column((int)SqlDbType.VarChar)]
    [AllowedToFilter]
    public string CashierName { get; set; } = default!;
    [Column((int)SqlDbType.VarChar)]
    [AllowedToFilter]
    [AllowedToOrder]
    public string ProductName { get; set; } = default!;
    [Column((int)SqlDbType.Decimal)]
    [AllowedToOrder]
    public decimal Price { get; set; }
}