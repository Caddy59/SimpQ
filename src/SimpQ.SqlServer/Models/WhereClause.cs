namespace SimpQ.SqlServer.Models;

/// <summary>
/// Represents the result of building a SQL WHERE clause, including the parameterized query string and its associated parameters.
/// </summary>
/// <param name="Query">The generated SQL WHERE clause.</param>
/// <param name="Parameters">The collection of parameters used within the WHERE clause.</param>
internal record WhereClause(string Query, IReadOnlyCollection<Parameter> Parameters);