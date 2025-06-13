namespace SimpQ.SqlServer.Queries.ClauseBuilders;

/// <summary>
/// Builds SQL Server-compatible pagination clauses using <c>OFFSET</c> and <c>FETCH NEXT</c>
/// for offset-based paging queries.
/// </summary>
internal static class PaginationClauseBuilder {
    /// <summary>
    /// Constructs an SQL pagination clause using <c>OFFSET</c>/<c>FETCH NEXT</c> syntax.
    /// </summary>
    /// <param name="offset">The number of rows to skip.</param>
    /// <param name="pageSize">The number of rows to retrieve after the offset.</param>
    /// <returns>A SQL clause string, e.g., <c>OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY;</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="offset"/> is less than 0 or <paramref name="pageSize"/> is less than or equal to 0.
    /// </exception>
    internal static string Build(int offset, int pageSize) {
        ArgumentOutOfRangeException.ThrowIfLessThan(offset, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pageSize, 0);
        return $"OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY;";
    }
}