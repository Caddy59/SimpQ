# SimpQ

**SimpQ** is a lightweight and extensible SQL filtering engine for .NET, focused on safety, readability, and ease of use. It enables dynamic filtering through structured JSON input and is designed to integrate seamlessly with SQL Server. The engine is easily adaptable to support other database providers.

This project was created to offer a simple and effective way to filter data in .NET applications without the complexity of traditional ORM frameworks.

## Features

- JSON-based query input model
- Offset-based pagination support
- Keyset-based pagination support
- SQL Server support, with extensibility to other relational databases

## Getting Started

### Installation

#### 1. Add the NuGet Package

Install **SimpQ** using the .NET CLI:

```bash
dotnet add package SimpQ.SqlServer
```

Or via the NuGet Package Manager in Visual Studio:

1.  **Right-click your project** → **Manage NuGet Packages**.  
2.  **Search for** `SimpQ.SqlServer`.  
3.  Click **Install**.

> ✅ SimpQ supports **.NET 8** and **.NET 9**.

#### 2. Register Services

In your `Program.cs`, configure **SimpQ** by registering its services in the dependency injection container.

Use the following to register the SQL Server implementation and provide the connection string:

```csharp
builder.Services.AddSimpQSqlServer(connectionString);
```

Next, register the `SimpQFilterJsonConverter` to enable deserialization of incoming filter requests via JSON. Add it to your JSON options configuration:

```csharp
builder.Services.AddControllers().AddJsonOptions(opt => {
    opt.JsonSerializerOptions.Converters.Add(new SimpQFilterJsonConverter());
});
```

Optionally, you can customize SimpQ's behavior — such as the maximum filter nesting level — by configuring the `SimpQOptions` class using either configuration binding or direct code-based setup:

```csharp
builder.Services.Configure<SimpQOptions>(
    builder.Configuration.GetSection("SimpQ"));
```

> The default value of `MaxFilterNestingLevel` is **2**. You can increase this limit if your application needs to support more deeply nested filter conditions.

#### 3. Define and Annotate Your Data Model

Your model class must implement the `IReportEntity` interface to be used with SimpQ.

Use attributes to:

- Declare SQL column types and names using `[Column]`
- Enable filtering and sorting using `[AllowedToFilter]` and `[AllowedToOrder]`
- Configure offset-based pagination using `[DefaultOrder]` to specify a default sort order
- Configure keyset-based pagination using `[KeysetPaginationKey]` to specify keyset pagination keys

Here’s a full example:

```csharp
public class Invoice : IReportEntity {
    [Column((int)SqlDbType.Int, name: "TransactionId")]
    [KeysetPaginationKey]
    [DefaultOrder]
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
```

> You can use both types of pagination in the same model.

#### 4. Use SimpQ in Your API

To execute queries using SimpQ, inject the `IReportQueryRaw` service into your controller and use either **offset-based** or **keyset-based** pagination depending on your scenario.

Below is an example `InvoiceController` that handles both types:

```csharp
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
            INNER JOIN InvoiceDetail id ON ih.Id = id.InvoiceHeaderId
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
            INNER JOIN InvoiceDetail id ON ih.Id = id.InvoiceHeaderId
        """;

        return await reportQueryRaw.ExecuteKeysetPagedQueryAsync<Invoice>(rawQuery, request, cancellationToken: cancellationToken);
    }
}
```

> You must ensure the SQL columns names and types match the model's `[Column]` attributes — otherwise, the mapping will fail at runtime.

#### 5. Sample JSON Request

SimpQ supports rich, structured JSON input for building dynamic filters. Below is an example using **offset-based pagination** and a **deeply nested filter expression** with logical OR conditions.

```json
{
  "filters": {
    "logic": "or",
    "conditions": [
      {
        "logic": "or",
        "conditions": [
          { "field": "CashierName", "operator": "equals", "value": "O'Reilly" },
          { "field": "CashierName", "operator": "equals", "value": "Alice" }
        ]
      },
      { "field": "CashierName", "operator": "equals", "value": "Bob" }
    ]
  },
  "order": [
    { "field": "CashierName", "direction": "desc" }
  ],
  "page": 1,
  "pageSize": 10000
}
```

> Ensure that all fields used in `filters` are annotated with `[AllowedToFilter]` in your model.
> Likewise, fields used in `order` must be annotated with `[AllowedToOrder]`.

## Special Thanks

- [Jacobo Requena](https://github.com/jrequenag) for his suggestions, ideas and support in this project.