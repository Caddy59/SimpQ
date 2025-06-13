namespace SimpQ.SqlServer.Queries.OperatorHandlers;

/// <summary>
/// Resolves the appropriate <see cref="IWhereOperatorHandler"/> based on the provided operator keyword.
/// Used to delegate SQL WHERE clause construction to the correct handler implementation.
/// </summary>
/// <param name="handlers">A collection of registered <see cref="IWhereOperatorHandler"/> implementations.</param>
public class WhereOperatorHandlerResolver(IEnumerable<IWhereOperatorHandler> handlers) {
    /// <summary>
    /// Finds the first handler that can process the given operator.
    /// </summary>
    /// <param name="operator">The filter operator keyword to resolve (e.g., "equals", "between").</param>
    /// <returns>
    /// An instance of <see cref="IWhereOperatorHandler"/> that supports the specified operator.
    /// </returns>
    /// <exception cref="InvalidOperatorException">
    /// Thrown if no registered handler supports the given operator.
    /// </exception>
    public IWhereOperatorHandler Resolve(string @operator) {
        var handler = handlers.FirstOrDefault(h => h.CanHandle(@operator));
        if (handler is null)
            throw new InvalidOperatorException(@operator, "filter");
        return handler;
    }
}