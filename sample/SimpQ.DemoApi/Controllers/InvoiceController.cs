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
        var rawQuery = """
            SELECT
                id.Id [TransactionId],
                ih.[Date],
                ih.CashierName,
                id.ProductName,
                id.Price
            FROM InvoiceHeader ih
            INNER JOIN InvoiceDetail id
            ON ih.Id = id.InvoiceHeaderId
            """;
        return await reportQueryRaw.ExecuteOffsetPagedQueryAsync<Invoice>(rawQuery, request, cancellationToken: cancellationToken);
    }

    [HttpPost("GetKeyset")]
    public async Task<KeysetPagedQueryResult<Invoice>> GetKeyset(KeysetPagedQueryParams request, CancellationToken cancellationToken) {
        var rawQuery = """
            SELECT
                id.Id [TransactionId],
                ih.[Date],
                ih.CashierName,
                id.ProductName,
                id.Price
            FROM InvoiceHeader ih
            INNER JOIN InvoiceDetail id
            ON ih.Id = id.InvoiceHeaderId
            """;
        return await reportQueryRaw.ExecuteKeysetPagedQueryAsync<Invoice>(rawQuery, request, cancellationToken: cancellationToken);
    }

    [HttpPost("GetOffsetFluentOnly")]
    public async Task<OffsetPagedQueryResult<InvoiceFluentOnly>> GetOffsetFluentOnly(OffsetPagedQueryParams request, CancellationToken cancellationToken) {
        var rawQuery = """
            SELECT
                id.Id [TransactionId],
                ih.[Date],
                ih.CashierName,
                id.ProductName,
                id.Price
            FROM InvoiceHeader ih
            INNER JOIN InvoiceDetail id
            ON ih.Id = id.InvoiceHeaderId
            """;
        return await reportQueryRaw.ExecuteOffsetPagedQueryAsync<InvoiceFluentOnly>(rawQuery, request, cancellationToken: cancellationToken);
    }

    [HttpPost("GetKeysetFluentOnly")]
    public async Task<KeysetPagedQueryResult<InvoiceFluentOnly>> GetKeysetFluentOnly(KeysetPagedQueryParams request, CancellationToken cancellationToken) {
        var rawQuery = """
            SELECT
                id.Id [TransactionId],
                ih.[Date],
                ih.CashierName,
                id.ProductName,
                id.Price
            FROM InvoiceHeader ih
            INNER JOIN InvoiceDetail id
            ON ih.Id = id.InvoiceHeaderId
            """;
        return await reportQueryRaw.ExecuteKeysetPagedQueryAsync<InvoiceFluentOnly>(rawQuery, request, cancellationToken: cancellationToken);
    }
}
