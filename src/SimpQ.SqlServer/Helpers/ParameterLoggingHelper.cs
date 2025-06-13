using System.Data;
using System.Text.Json;

namespace SimpQ.SqlServer.Helpers;

internal static class ParameterLoggingHelper {
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() {
        WriteIndented = true
    };

    internal static string ToJsonWithSqlDbType(IEnumerable<Parameter> parameters) {
        var projection = parameters.Select(p => new {
            p.Name,
            p.Value,
            DbType = Enum.IsDefined(typeof(SqlDbType), p.DbType)
                ? ((SqlDbType)p.DbType).ToString()
                : $"Unknown({p.DbType})"
        });

        return JsonSerializer.Serialize(projection, _jsonSerializerOptions);
    }
}