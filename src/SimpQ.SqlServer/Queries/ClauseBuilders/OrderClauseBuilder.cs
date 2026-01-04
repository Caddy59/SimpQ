using SimpQ.Abstractions.Attributes.Entities;
using SimpQ.Abstractions.Enums;
using SimpQ.Core.Configuration;
using SimpQ.Core.Helpers;

namespace SimpQ.SqlServer.Queries.ClauseBuilders;

/// <summary>
/// Builds SQL <c>ORDER BY</c> clauses for SimpQ queries, supporting both offset-based and keyset-based pagination.
/// </summary>
/// <param name="sqlServerQueryOperator">
/// An instance of <see cref="SqlServerQueryOperator"/> used to resolve SQL ordering directions.
/// </param>
/// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
public class OrderClauseBuilder(SqlServerQueryOperator sqlServerQueryOperator, EntityConfigurationRegistry? configurationRegistry = null)  {
    private const string Separator = ",";

    /// <summary>
    /// Constructs a complete <c>ORDER BY</c> clause based on explicit ordering or keyset pagination metadata.
    /// </summary>
    /// <typeparam name="TEntity">The entity type defining orderable fields.</typeparam>
    /// <param name="order">Optional list of ordering instructions (field + direction).</param>
    /// <param name="keysetFilter">Optional keyset filter that enables keyset pagination logic.</param>
    /// <returns>A SQL <c>ORDER BY</c> clause, or an empty string if none is required.</returns>
    /// <exception cref="InvalidColumException">Thrown if an ordering field is not allowed for ordering.</exception>
    /// <exception cref="InvalidOrderDirectionException">Thrown if the direction string is invalid or unsupported.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if keyset columns are explicitly reused in an <paramref name="order"/> if using keyset pagination.
    /// </exception>
    internal string Build<TEntity>(IReadOnlyCollection<Order>? order, KeysetFilter? keysetFilter) where TEntity : IReportEntity, new() {
        var clause = keysetFilter is not null
            ? CombineOrderClause(GetColumns<TEntity>(order, isKeysetPagination: true), GetKeysetOrderClause<TEntity>(order, keysetFilter.IsDescending))
            : GetColumns<TEntity>(order);

        return string.IsNullOrWhiteSpace(clause) ? string.Empty : $"ORDER BY{Environment.NewLine}{clause}";
    }

    /// <summary>
    /// Builds the SQL fragment based on allowed columns and user-provided <paramref name="order"/> list.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to extract allowed columns from.</typeparam>
    /// <param name="order">Optional list of user-defined order fields and directions.</param>
    /// <param name="isKeysetPagination">Indicates if this is part of a keyset pagination scenario (optiomal). Default is <c>false</c>.</param>
    /// <returns>Formatted <c>ORDER BY</c> fragment or default ordering when applicable.</returns>
    private string GetColumns<TEntity>(IReadOnlyCollection<Order>? order, bool isKeysetPagination = false) where TEntity : IReportEntity, new() {
        var allowedColumns = QueryHelper.GetAllowedColumnsToOrder<TEntity>(configurationRegistry);
        var useDefaultOrder = order is null || order.Count == 0;
        
        if (useDefaultOrder && !isKeysetPagination)
            return GetDefaultOrderClause<TEntity>();
        if (useDefaultOrder && isKeysetPagination)
            return string.Empty;

        var orderFields = order!.Select(c => {
            var allowedColumn = allowedColumns
                .SingleOrDefault(x => x.PropertyName.Equals(c.Field, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidColumException(c.Field, "order");

            var direction = c.Direction;
            if (!SqlServerAllowedOperator.OrderingOperators.TryGetValue(direction, out var sqlOrderDirection))
                throw new InvalidOrderDirectionException(allowedColumn.PropertyName, direction);

            return $"{allowedColumn.Name.EscapeColumnName()} {sqlOrderDirection}";
        });

        var formattedOrder = orderFields.Select((s, i) => i == 0 ? s : Separator + s);
        return string.Join(Environment.NewLine, formattedOrder);
    }

    /// <summary>
    /// Constructs the ordering clause from keyset pagination metadata.
    /// </summary>
    /// <typeparam name="TEntity">The entity type defining keyset columns.</typeparam>
    /// <param name="order">Optional user-defined ordering (should not conflict with keyset columns).</param>
    /// <param name="isDescending">Indicates whether keyset pagination should order descending.</param>
    /// <returns>A clause built from keyset-prioritized columns and direction.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if any keyset pagination column is also present in <paramref name="order"/>.
    /// </exception>
    private string GetKeysetOrderClause<TEntity>(IReadOnlyCollection<Order>? order, bool isDescending) where TEntity : IReportEntity, new() {
        var keysetColumns = QueryHelper.GetOrderedKeysetColumns<TEntity>(configurationRegistry);
        var keysetDirection = isDescending ? OrderDirection.Descending : OrderDirection.Ascending;

        if (order is not null && order.Count > 0) {
            var keysetPropertyNames = keysetColumns
               .Select(c => c.PropertyName)
               .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var conflictingFields = order
                .Select(o => o.Field)
                .Where(keysetPropertyNames.Contains)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (conflictingFields.Length > 0)
                throw new InvalidOperationException($"Cannot use keyset column(s) [{string.Join(", ", conflictingFields)}] in order clause when using keyset pagination.");
        }

        var formattedOrder = keysetColumns
            .Select((c, i) =>
                (i == 0 ? string.Empty : Separator) +
                $"{c.Name.EscapeColumnName()} {GetOrderDirection(keysetDirection)}");

        return string.Join(Environment.NewLine, formattedOrder);
    }

    /// <summary>
    /// Builds the default order clause from columns annotated with <see cref="DefaultOrderAttribute"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type defining the default order strategy.</typeparam>
    /// <returns>A formatted default <c>ORDER BY</c> clause.</returns>
    private string GetDefaultOrderClause<TEntity>() where TEntity : IReportEntity, new() {
        var defaultOrders = QueryHelper.GetDefaultColumnsToOrder<TEntity>(configurationRegistry);

        var formattedOrder = defaultOrders
            .OrderBy(c => c.Priority)
            .Select((c, i) =>
                (i == 0 ? string.Empty : Separator) +
                $"{c.Name.EscapeColumnName()} {GetOrderDirection(c.Direction)}");

        return string.Join(Environment.NewLine, formattedOrder);
    }

    /// <summary>
    /// Converts the enum-based <see cref="OrderDirection"/> to the SQL literal keyword.
    /// </summary>
    /// <param name="orderDirection">The logical order direction.</param>
    /// <returns><c>ASC</c> or <c>DESC</c>.</returns>
    /// <exception cref="NotSupportedException">Thrown if the direction is not supported.</exception>
    private string GetOrderDirection(OrderDirection orderDirection) {
        return orderDirection switch {
            OrderDirection.Descending => sqlServerQueryOperator.Descending,
            OrderDirection.Ascending => sqlServerQueryOperator.Ascending,
            _ => throw new NotSupportedException()
        };
    }

    /// <summary>
    /// Combines two ORDER BY clause fragments into one.
    /// </summary>
    /// <param name="baseClause">The first part of the ordering clause (user or default).</param>
    /// <param name="keysetClause">The appended keyset ordering fragment.</param>
    /// <returns>The combined clause with proper separator and line breaks.</returns>
    private static string CombineOrderClause(string baseClause, string keysetClause) =>
        string.IsNullOrWhiteSpace(baseClause)
            ? keysetClause
            : string.Join($"{Environment.NewLine}{Separator}", baseClause, keysetClause);
}