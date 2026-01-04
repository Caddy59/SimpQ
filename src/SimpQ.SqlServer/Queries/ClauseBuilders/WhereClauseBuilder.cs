using Microsoft.Extensions.Options;
using SimpQ.Core.Contexts;
using SimpQ.Core.Options;
using SimpQ.SqlServer.Models;
using SimpQ.SqlServer.Queries.OperatorHandlers;
using System.Text.Json;
using SimpQ.Core.Configuration;
using SimpQ.Core.Helpers;

namespace SimpQ.SqlServer.Queries.ClauseBuilders;

/// <summary>
/// Builds SQL <c>WHERE</c> clauses based on SimpQ filter definitions, supporting both traditional and keyset-based pagination filtering.
/// </summary>
/// <param name="options">
/// The SimpQ configuration options, including the maximum allowed nesting depth for filter groups.
/// Provided via dependency injection using <see cref="Microsoft.Extensions.Options.IOptions{TOptions}"/>.
/// </param>
/// <param name="validOperator">The validator for checking if an operator is valid for a property type.</param>
/// <param name="simpQOperator">The canonical SimpQ operator keywords used in input filters.</param>
/// <param name="handlerResolver">Resolves the correct handler based on the filter operator.</param>
/// <param name="configurationRegistry">Optional configuration registry for fluent configurations.</param>
public class WhereClauseBuilder(IOptions<SimpQOptions> options, ValidOperator validOperator, SimpQOperator simpQOperator, WhereOperatorHandlerResolver handlerResolver, EntityConfigurationRegistry? configurationRegistry = null) {
    private readonly byte _maxNestingLevel = options.Value.MaxFilterNestingLevel;

    /// <summary>
    /// Builds the SQL <c>WHERE</c> clause for a given filter and optional keyset pagination.
    /// </summary>
    /// <typeparam name="TEntity">The report entity type used to validate filter fields.</typeparam>
    /// <param name="filters">A group of filter conditions to convert to SQL.</param>
    /// <param name="keysetFilter">Optional keyset filter applied for pagination.</param>
    /// <param name="parameters">Outputs the generated SQL parameters.</param>
    /// <returns>The formatted <c>WHERE</c> clause, or an empty string if no filters apply.</returns>
    /// <exception cref="InvalidColumException">Thrown when a filtered field is not allowed.</exception>
    /// <exception cref="InvalidOperatorException">Thrown when an operator is not valid for the field's type.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when conflicting keyset fields are found in the filter group, or when nesting exceeds the maximum nesting level.
    /// </exception>
    internal string Build<TEntity>(FilterGroup? filters, KeysetFilter? keysetFilter, out IReadOnlyCollection<Parameter> parameters) where TEntity : IReportEntity {
        var where = keysetFilter is not null
            ? GetKeysetClause<TEntity>(filters, keysetFilter)
            : GetClause<TEntity>(filters);
        parameters = where.Parameters;

        return string.IsNullOrWhiteSpace(where.Query)
            ? string.Empty
            : $"WHERE{Environment.NewLine}{where.Query}";
    }

    /// <summary>
    /// Builds a basic <c>WHERE</c> clause from the provided filter group.
    /// </summary>
    /// <typeparam name="TEntity">Entity type used to validate fields.</typeparam>
    /// <param name="filterGroup">The filter group to translate into SQL.</param>
    /// <returns>A <see cref="WhereClause"/> with SQL and parameter list.</returns>
    private WhereClause GetClause<TEntity>(FilterGroup? filterGroup) where TEntity : IReportEntity {
        if (filterGroup is null || filterGroup.Conditions.Count == 0)
            return new WhereClause(string.Empty, []);

        var parameterContext = new ParameterContext();
        var clause = BuildClauseGroup<TEntity>(filterGroup, parameterContext);
        return new WhereClause(clause, [.. parameterContext.Parameters]);
    }

    /// <summary>
    /// Builds a <c>WHERE</c> clause that includes a keyset pagination filter and validates for conflicts.
    /// </summary>
    /// <typeparam name="TEntity">Entity type used to retrieve keyset columns.</typeparam>
    /// <param name="filterGroup">Optional filter group used along with keyset filters.</param>
    /// <param name="keysetFilter">The keyset filter with column values.</param>
    /// <returns>A <see cref="WhereClause"/> containing the SQL and parameters.</returns>
    /// <exception cref="InvalidOperationException">Thrown if keyset columns are also used in the filter group.</exception>
    private WhereClause GetKeysetClause<TEntity>(FilterGroup? filterGroup, KeysetFilter keysetFilter) where TEntity : IReportEntity {
        var keysetColumns = QueryHelperExtensions.GetOrderedKeysetColumns<TEntity>(configurationRegistry)
            .Select(c => c.PropertyName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (filterGroup is not null) {
            var fields = GetAllFilterFields(filterGroup);
            var conflictingFields = fields
                .Where(keysetColumns.Contains)
                .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

            if (conflictingFields.Length > 0)
                throw new InvalidOperationException($"Cannot use keyset column(s) [{string.Join(", ", conflictingFields)}] in filter clause when using keyset pagination.");
        }

        var parameterContext = new ParameterContext();
        var clause = string.Empty;
        if (filterGroup is not null && filterGroup.Conditions.Count > 0)
            clause = BuildClauseGroup<TEntity>(filterGroup, parameterContext);

        var keysetClause = GetKeysetClause<TEntity>(keysetFilter, parameterContext);
        var andSqlOperator = SqlServerAllowedOperator.LogicalOperators[simpQOperator.And];
        var combinedClause = string.Join($"{Environment.NewLine}{andSqlOperator} ", new[] { clause, keysetClause }.Where(c => !string.IsNullOrWhiteSpace(c)));

        return new WhereClause(combinedClause, [.. parameterContext.Parameters]);
    }

    /// <summary>
    /// Recursively builds SQL filter conditions from a filter group, enforcing a max depth of 2.
    /// </summary>
    /// <typeparam name="TEntity">The entity being queried.</typeparam>
    /// <param name="group">Filter group to process.</param>
    /// <param name="parameterContext">Parameter context used to store SQL parameters.</param>
    /// <param name="level">Current nesting level for validation.</param>
    /// <returns>SQL fragment representing the group.</returns>
    /// <exception cref="InvalidOperatorException">Thrown if a logical operator is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown if nesting exceeds the maximum nesting level.</exception>
    private string BuildClauseGroup<TEntity>(FilterGroup group, ParameterContext parameterContext, int level = 1) where TEntity : IReportEntity {
        if (level > _maxNestingLevel)
            throw new InvalidOperationException($"Maximum filter nesting level exceeded (maximum level: {_maxNestingLevel}).");

        if (!SqlServerAllowedOperator.LogicalOperators.TryGetValue(group.Logic, out var sqlLogic))
            throw new InvalidOperatorException(group.Logic, "logical");

        var clauses = new List<string>();

        foreach (var filter in group.Conditions) {
            var clause = BuildClause<TEntity>(filter, parameterContext, level);
            if (!string.IsNullOrWhiteSpace(clause))
                clauses.Add(clause);
        }

        return string.Join(Environment.NewLine + sqlLogic + " ", clauses);
    }

    /// <summary>
    /// Builds a SQL clause from a single filter or filter group.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being filtered.</typeparam>
    /// <param name="filter">The filter to translate.</param>
    /// <param name="parameterContext">Parameter context used to store SQL parameters.</param>
    /// <param name="level">Current nesting level for validation.</param>
    /// <returns>A valid SQL condition string or empty.</returns>
    private string BuildClause<TEntity>(IFilter filter, ParameterContext parameterContext, int level) where TEntity : IReportEntity {
        var allowedFilters = QueryHelperExtensions.GetAllowedColumnsToFilter<TEntity>(configurationRegistry);

        if (filter is FilterGroup subgroup)
            return $"({BuildClauseGroup<TEntity>(subgroup, parameterContext, level + 1)})";

        if (filter is FilterCondition condition) {
            var column = allowedFilters.SingleOrDefault(c => c.PropertyName.Equals(condition.Field, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidColumException(condition.Field, "filter");

            if (!validOperator.IsOperatorValidForType(column.PropertyType, condition.Operator))
                throw new InvalidOperatorException(condition.Operator, "comparison", column.PropertyName, column.PropertyType, validOperator.GetOperatorsForType(column.PropertyType));

            var handler = handlerResolver.Resolve(condition.Operator);
            return handler.BuildClause(column.Name, column.DbType, condition.Operator, JsonSerializer.SerializeToElement(condition.Value), parameterContext);
        }

        return string.Empty;
    }

    /// <summary>
    /// Builds SQL filter expressions for keyset-based pagination.
    /// </summary>
    /// <typeparam name="TEntity">Entity defining the keyset columns.</typeparam>
    /// <param name="keysetFilter">The keyset filter with column values.</param>
    /// <param name="parameterContext">Parameter context used to store SQL parameters.</param>
    /// <returns>SQL clause string combining comparisons of keyset fields.</returns>
    /// <exception cref="InvalidOperationException">If required keyset values are missing.</exception>
    private string GetKeysetClause<TEntity>(KeysetFilter keysetFilter, ParameterContext parameterContext) where TEntity : IReportEntity {
        if (keysetFilter.Keyset is null || keysetFilter.Keyset.Count == 0)
            return string.Empty;

        var @operator = keysetFilter.IsDescending ? simpQOperator.LessThan : simpQOperator.GreaterThan;
        var columns = QueryHelperExtensions.GetOrderedKeysetColumns<TEntity>(configurationRegistry);
        var conditions = new List<string>();

        for (var i = 0; i < columns.Count; i++) {
            var parts = new List<string>();

            for(var j = 0; j < i; j++) {
                var previousColumn = columns.ElementAt(j);

                if (!keysetFilter.Keyset.TryGetValue(previousColumn.PropertyName, out var previousValue))
                    throw new InvalidOperationException($"Missing keyset value for column '{previousColumn.PropertyName}'.");

                var equalsHandler = handlerResolver.Resolve(simpQOperator.Equals);
                parts.Add(equalsHandler.BuildClause(previousColumn.Name, previousColumn.DbType, simpQOperator.Equals, JsonSerializer.SerializeToElement(previousValue), parameterContext));
            }

            var column = columns.ElementAt(i);
            if (!keysetFilter.Keyset.TryGetValue(column.PropertyName, out var comparisonValue))
                throw new InvalidOperationException($"Missing keyset value for column '{column.PropertyName}'.");
            
            var comparisonHandler = handlerResolver.Resolve(@operator);
            parts.Add(comparisonHandler.BuildClause(column.Name, column.DbType, @operator, JsonSerializer.SerializeToElement(comparisonValue), parameterContext));
            conditions.Add("(" + string.Join($" {SqlServerAllowedOperator.LogicalOperators[simpQOperator.And]} ", parts) + ")");
        }

        var keysetClause = string.Join($" {SqlServerAllowedOperator.LogicalOperators[simpQOperator.Or]} ", conditions);
        return $"({keysetClause})";
    }

    /// <summary>
    /// Extracts all field names referenced by a filter tree (group or condition).
    /// </summary>
    /// <param name="filter">The root filter to inspect.</param>
    /// <returns>An enumerable of distinct field names.</returns>
    private static IEnumerable<string> GetAllFilterFields(IFilter filter) {
        if (filter is FilterCondition condition)
            yield return condition.Field;
        if (filter is FilterGroup group) {
            foreach (var child in group.Conditions)
                foreach (var field in GetAllFilterFields(child))
                    yield return field;
        }
    }
}