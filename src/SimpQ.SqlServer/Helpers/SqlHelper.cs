namespace SimpQ.SqlServer.Helpers;

/// <summary>
/// Provides utility methods for working with SQL Server specific syntax and formatting.
/// </summary>
internal static class SqlHelper {
    /// <summary>
    /// Escapes a SQL column name by wrapping it in square brackets to prevent conflicts with reserved keywords or special characters.
    /// </summary>
    /// <param name="columnName">The name of the column to escape.</param>
    /// <returns>The escaped column name, e.g., <c>[ColumnName]</c>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="columnName"/> is null, empty, or consists only of whitespace.</exception>
    internal static string EscapeColumnName(this string columnName) {
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);
        return $"[{columnName}]";
    }
}