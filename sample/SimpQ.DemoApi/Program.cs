using SimpQ.Core.Options;
using SimpQ.Core.Serialization;
using SimpQ.SqlServer.Extensions;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SimpQDb");
ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

builder.Services.AddControllers().AddJsonOptions(opt => {
    opt.JsonSerializerOptions.Converters.Add(new SimpQFilterJsonConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSimpQSqlServer(connectionString);

builder.Services.Configure<SimpQOptions>(
    builder.Configuration.GetSection("SimpQ"));

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("Running on framework {0}", RuntimeInformation.FrameworkDescription);

await app.RunAsync();