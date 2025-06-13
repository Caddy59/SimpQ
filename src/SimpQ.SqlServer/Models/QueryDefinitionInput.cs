namespace SimpQ.SqlServer.Models;

/// <summary>
/// Represents the input required to build a SQL query definition, including clauses for selection,
/// filtering, ordering, and pagination. Used internally by SimpQ to compose parameterized SQL commands.
/// </summary>
/// <param name="RawQuery">The base query or SQL table expression (typically a <c>FROM</c> clause).</param>
/// <param name="RawInitSql">Optional pre-query SQL (e.g., CTEs, temporary tables) to be prepended to the query.</param>
/// <param name="Select">Optional list of fields to include in the <c>SELECT</c> clause. If <c>null</c>, all allowed columns will be selected.</param>
/// <param name="Filters">Optional filter group used to construct the <c>WHERE</c> clause.</param>
/// <param name="Order">Optional list of sort rules for the <c>ORDER BY</c> clause.</param>
/// <param name="AppendCountQuery">Whether to generate a count query alongside the data query (used in offset pagination).</param>
/// <param name="PaginationClause">The pagination clause (e.g., <c>OFFSET ... FETCH</c> or empty if not paginated).</param>
/// <param name="KeysetFilter">Optional keyset pagination values to append to the <c>WHERE</c> clause.</param>
internal record QueryDefinitionInput(string RawQuery, string RawInitSql, IReadOnlyCollection<Select>? Select, FilterGroup? Filters, IReadOnlyCollection<Order>? Order, bool AppendCountQuery = false, string PaginationClause = "", KeysetFilter? KeysetFilter = null);