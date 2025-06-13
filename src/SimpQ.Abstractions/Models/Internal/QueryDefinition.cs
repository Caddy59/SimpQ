namespace SimpQ.Abstractions.Models.Internal;

/// <summary>
/// Represents the definition of a database query, including the SQL commands for fetching rows and counting total results,
/// along with the parameters required by both.
/// </summary>
/// <param name="RowsCommand">The SQL command used to retrieve the paginated or filtered result set.</param>
/// <param name="CountCommand">The SQL command used to retrieve the total number of matching rows (without pagination).</param>
/// <param name="Parameters">The parameters to be applied to both the row and count commands.</param>
public record QueryDefinition(string RowsCommand, string CountCommand, IReadOnlyCollection<Parameter> Parameters);