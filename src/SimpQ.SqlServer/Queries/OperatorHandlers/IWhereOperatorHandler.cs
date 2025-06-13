using SimpQ.Core.Contexts;
using System.Text.Json;

namespace SimpQ.SqlServer.Queries.OperatorHandlers;

/// <summary>
/// Defines the contract for building SQL WHERE clause fragments for a specific comparison operator.
/// Implementations of this interface are responsible for validating and translating operators into SQL syntax,
/// including parameterization.
/// </summary>
public interface IWhereOperatorHandler {
    /// <summary>
    /// Determines whether this handler can process the specified operator keyword.
    /// </summary>
    /// <param name="operator">The operator keyword to check (e.g., "equals", "between").</param>
    /// <returns><c>true</c> if the handler supports the operator; otherwise, <c>false</c>.</returns>
    bool CanHandle(string @operator);

    /// <summary>
    /// Builds a SQL WHERE clause fragment for the specified operator, column, and value.
    /// The generated SQL must use parameters from the provided <paramref name="parameterContext"/>.
    /// </summary>
    /// <param name="columnName">The database column name to apply the filter on.</param>
    /// <param name="dbType">The underlying SQL database type of the column (used for parameter binding).</param>
    /// <param name="operator">The operator keyword (e.g., "greater", "in", "contains").</param>
    /// <param name="value">The JSON value to be used in the filter (single or array, depending on operator).</param>
    /// <param name="parameterContext">The context used to register and generate parameter names.</param>
    /// <returns>A SQL fragment (e.g., <c>"[Amount] > @p0"</c>) ready to be included in a WHERE clause.</returns>
    string BuildClause(string columnName, int dbType, string @operator, JsonElement value, ParameterContext parameterContext);
}