using Microsoft.AspNetCore.Mvc;
using SimpQ.Abstractions.Models.Requests;
using SimpQ.Abstractions.Models.Results;
using SimpQ.Abstractions.Reports;
using SimpQ.DemoApi.ReportEntities;

namespace SimpQ.DemoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class InvoiceController(IReportQueryRaw reportQueryRaw) : ControllerBase {
    [HttpPost("GetOffset")]
    public async Task<OffsetPagedQueryResult<Invoice>> GetOffset(OffsetPagedQueryParams request, CancellationToken cancellationToken) {
        var rawQuery = "SELECT\r\n id.Id [TransactionId]\r\n,ih.[Date]\r\n,ih.CashierName\r\n,id.ProductName\r\n,id.Price\r\nFROM InvoiceHeader ih\r\nINNER JOIN InvoiceDetail id\r\nON ih.Id = id.InvoiceHeaderId";
        return await reportQueryRaw.ExecuteOffsetPagedQueryAsync<Invoice>(rawQuery, request, cancellationToken: cancellationToken);
    }

    [HttpPost("GetKeyset")]
    public async Task<KeysetPagedQueryResult<Invoice>> GetKeyset(KeysetPagedQueryParams request, CancellationToken cancellationToken) {
        var rawQuery = "SELECT\r\n id.Id [TransactionId]\r\n,ih.[Date]\r\n,ih.CashierName\r\n,id.ProductName\r\n,id.Price\r\nFROM InvoiceHeader ih\r\nINNER JOIN InvoiceDetail id\r\nON ih.Id = id.InvoiceHeaderId";
        return await reportQueryRaw.ExecuteKeysetPagedQueryAsync<Invoice>(rawQuery, request, cancellationToken: cancellationToken);
    }
}
