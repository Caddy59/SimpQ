using SimpQ.Abstractions.Attributes.Entities;
using SimpQ.Core.Configuration;
using SimpQ.Core.Helpers;

namespace SimpQ.SqlServer.Queries.ClauseBuilders;

/// <summary>
/// Builds the SELECT clause for SimpQ queries, validating that only allowed columns are included,
/// and generating proper SQL syntax with optional user-defined field selection.
/// </summary>
internal static class SelectClauseBuilder {
    private const string Separator = ",";

    /// <summary>
    /// Constructs a complete SELECT clause using allowed columns defined via <seea cref="ColumnAttribute"/>.
    /// If no specific fields are provided, all mapped columns are selected.
    /// </summary>
    /// <typeparam name="TEntity">The entity type that defines allowed columns. Must implement <see cref="IReportEntity"/>.</typeparam>
    /// <param name="select">An optional list of user-selected fields (by property name).</param>
    /// <param name="rawQuery">The raw SQL fragment (e.g., a FROM clause or CTE) to wrap in a derived table.</param>
    /// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
    /// <returns>A complete SELECT clause ending with a derived table alias, e.g., <c>FROM (...rawQuery...) "result"</c>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="rawQuery"/> is null or whitespace.</exception>
    /// <exception cref="InvalidColumException">Thrown if a requested field does not match any allowed property in <typeparamref name="TEntity"/>.</exception>
    internal static string Build<TEntity>(IReadOnlyCollection<Select>? select, string rawQuery, ReportEntityConfigurationRegistry? configurationRegistry = null) where TEntity : IReportEntity {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawQuery);
        return $"SELECT{Environment.NewLine}{GetSentence<TEntity>(select, configurationRegistry)}{Environment.NewLine}FROM ({rawQuery}) \"result\"";
    }

    /// <summary>
    /// Builds the list of columns to select based on allowed properties and the optional user-defined selection.
    /// </summary>
    /// <typeparam name="TEntity">The entity type used to validate and resolve property mappings.</typeparam>
    /// <param name="select">The optional list of selected property names.</param>
    /// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
    /// <returns>A formatted multi-line string of escaped column names, separated by commas.</returns>
    /// <exception cref="InvalidColumException">
    /// Thrown when a field in <paramref name="select"/> is not decorated with <see cref="ColumnAttribute"/> or is not allowed.
    /// </exception>
    private static string GetSentence<TEntity>(IReadOnlyCollection<Select>? select, ReportEntityConfigurationRegistry? configurationRegistry = null) where TEntity : IReportEntity {
        var allowedColumns = QueryHelper.GetColumns<TEntity>(configurationRegistry);

        if (select is null || select.Count == 0) {
            var allColumns = allowedColumns
                .Select(c => c.Name.EscapeColumnName())
                .Select((c, i) => i == 0 ? c : Separator + c);

            return string.Join(Environment.NewLine, allColumns);
        }

        var selectedFields = select.Select(s => {
            var allowedColumn = allowedColumns
                .SingleOrDefault(x => x.PropertyName.Equals(s.Field, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidColumException(s.Field, "select");

            return allowedColumn.Name.EscapeColumnName();
        });

        var formattedSelect = selectedFields.Select((c, i) => i == 0 ? c : Separator + c);
        return string.Join(Environment.NewLine, formattedSelect);
    }
}