using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace SimpQ.SqlServer.Helpers;

/// <summary>
/// Provides helper methods for mapping SQL result sets to report entities and for building SQL parameters.
/// </summary>
internal static class EntityHelper {
    /// <summary>
    /// Maps the current row of a <see cref="IDataReader"/> to an instance of <typeparamref name="TEntity"/> 
    /// by matching column names to property names defined in the entity metadata.
    /// </summary>
    /// <typeparam name="TEntity">The target entity type. Must implement <see cref="IReportEntity"/> and have a parameterless constructor.</typeparam>
    /// <param name="dataReader">The <see cref="IDataReader"/> containing the query result.</param>
    /// <returns>An instance of <typeparamref name="TEntity"/> populated with values from the current row.</returns>
    internal static TEntity GetEntity<TEntity>(this IDataReader dataReader) where TEntity : IReportEntity, new() {
        var columns = QueryHelper.GetColumns<TEntity>();
        var entity = new TEntity();
        var entityType = typeof(TEntity);

        for (int i = 0; i < dataReader.FieldCount; i++) {
            if (dataReader.IsDBNull(i))
                continue;

            var columnName = dataReader.GetName(i);
            var columnValue = dataReader.GetValue(i)!;
            var column = columns.SingleOrDefault(c => c.Name == columnName);
            if (column is null)
                continue;

            var propertyInfo = entityType.GetProperty(column.PropertyName);
            if (propertyInfo is null || !propertyInfo.CanWrite)
                continue;

            var convertedValue = TryConvert(column.PropertyType, columnValue);
            if (convertedValue is not null)
                propertyInfo.SetValue(entity, convertedValue);
        }

        return entity;
    }

    /// <summary>
    /// Attempts to convert the given value to the specified target type, handling special cases such as <see cref="DateOnly"/> and <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="targetType">The target property type to convert to, which may be nullable.</param>
    /// <param name="value">The value to be converted, typically read from a data reader.</param>
    /// <returns>
    /// The converted value if successful; otherwise, <c>null</c> if the conversion fails.
    /// </returns>
    private static object? TryConvert(Type targetType, object value) {
        var actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        try {
            return (actualType, value) switch {
                (Type type, DateTime dateTime) when type == typeof(DateOnly) => DateOnly.FromDateTime(dateTime),
                (Type type, TimeSpan timeSpan) when type == typeof(TimeOnly) => TimeOnly.FromTimeSpan(timeSpan),
                (_, _) => Convert.ChangeType(value, actualType, CultureInfo.InvariantCulture)
            };
        }
        catch {
            // Optionally log or rethrow depending on your needs
            return null;
        }
    }

    /// <summary>
    /// Converts an array of <see cref="Parameter"/> objects into an array of <see cref="SqlParameter"/> objects,
    /// suitable for use with SQL Server commands.
    /// </summary>
    /// <param name="parameters">The array of SimpQ parameters to convert.</param>
    /// <returns>An array of <see cref="SqlParameter"/> instances configured for input.</returns>
    internal static SqlParameter[] GetSqlParameters(IReadOnlyCollection<Parameter> parameters) {
        return [.. parameters
            .Select(p => {
                var sqlParameter = new SqlParameter {
                    ParameterName = p.Name,
                    Direction = ParameterDirection.Input,
                    Value = p.Value,
                    SqlDbType = p.DbType > -1 ? (SqlDbType)p.DbType : SqlDbType.VarChar
                };

                return sqlParameter;
            })];
    }
}