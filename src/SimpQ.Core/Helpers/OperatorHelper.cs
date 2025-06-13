using SimpQ.Abstractions.Attributes.Operators;
using SimpQ.Abstractions.Queries;
using System.Collections.Frozen;
using System.Reflection;

namespace SimpQ.Core.Helpers;

/// <summary>
/// Provides utility methods for extracting categorized query operators
/// (comparison, logical, ordering) from implementations of <see cref="IQueryOperator"/> using attribute-based filtering.
/// </summary>
public static class OperatorHelper {
    /// <summary>
    /// Retrieves a dictionary of comparison operators (e.g., "equals", "greater") defined in a given <see cref="IQueryOperator"/> implementation.
    /// </summary>
    /// <typeparam name="TQueryOperatorKey">The operator type used as the dictionary keys (e.g., from user input).</typeparam>
    /// <typeparam name="TQueryOperatorValue">The operator type used as the dictionary values (e.g., for SQL translation).</typeparam>
    /// <returns>A case-insensitive frozen dictionary mapping comparison operator keys to their values.</returns>
    public static FrozenDictionary<string, string> GetComparisonOperators<TQueryOperatorKey, TQueryOperatorValue>() where TQueryOperatorKey : IQueryOperator, new()
        where TQueryOperatorValue : IQueryOperator, new() => GetAllowedOperators<TQueryOperatorKey, TQueryOperatorValue, ComparisonOperatorAttribute>();

    /// <summary>
    /// Retrieves a dictionary of logical operators (e.g., "and", "or") defined in a given <see cref="IQueryOperator"/> implementation.
    /// </summary>
    /// <typeparam name="TQueryOperatorKey">The operator type used as the dictionary keys (e.g., from user input).</typeparam>
    /// <typeparam name="TQueryOperatorValue">The operator type used as the dictionary values (e.g., for internal logic).</typeparam>
    /// <returns>A case-insensitive frozen dictionary mapping logical operator keys to their values.</returns>
    public static FrozenDictionary<string, string> GetLogicalOperators<TQueryOperatorKey, TQueryOperatorValue>() where TQueryOperatorKey : IQueryOperator, new()
        where TQueryOperatorValue : IQueryOperator, new() => GetAllowedOperators<TQueryOperatorKey, TQueryOperatorValue, LogicalOperatorAttribute>();

    /// <summary>
    /// Retrieves a dictionary of ordering operators (e.g., "asc", "desc") defined in a given <see cref="IQueryOperator"/> implementation.
    /// </summary>
    /// <typeparam name="TQueryOperatorKey">The operator type used as the dictionary keys (e.g., from user input).</typeparam>
    /// <typeparam name="TQueryOperatorValue">The operator type used as the dictionary values (e.g., for SQL translation).</typeparam>
    /// <returns>A case-insensitive frozen dictionary mapping ordering operator keys to their values.</returns>
    public static FrozenDictionary<string, string> GetOrderingOperators<TQueryOperatorKey, TQueryOperatorValue>() where TQueryOperatorKey : IQueryOperator, new()
        where TQueryOperatorValue : IQueryOperator, new() => GetAllowedOperators<TQueryOperatorKey, TQueryOperatorValue, OrderingOperatorAttribute>();

    /// <summary>
    /// Uses reflection to extract properties from <see cref="IQueryOperator"/> that are decorated with the specified attribute
    /// and builds a frozen dictionary mapping key values to corresponding translated values.
    /// </summary>
    /// <typeparam name="TQueryOperatorKey">The operator type used for keys.</typeparam>
    /// <typeparam name="TQueryOperatorValue">The operator type used for values.</typeparam>
    /// <typeparam name="TAttribute">The attribute used to identify relevant properties.</typeparam>
    /// <returns>A frozen dictionary of operator keys and their corresponding values.</returns>
    private static FrozenDictionary<string, string> GetAllowedOperators<TQueryOperatorKey, TQueryOperatorValue, TAttribute>() where TQueryOperatorKey : IQueryOperator, new()
        where TQueryOperatorValue : IQueryOperator, new()
        where TAttribute : Attribute {
        var operatorKey = new TQueryOperatorKey();
        var operatorValue = new TQueryOperatorValue();

        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var properties = typeof(IQueryOperator).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<TAttribute>() is not null);

        foreach(var property in properties) {
            var key = (string)property.GetValue(operatorKey)!;
            var value = (string)property.GetValue(operatorValue)!;
            dict.Add(key, value);
        }

        return dict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }
}