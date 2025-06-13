using SimpQ.Abstractions.Models.Internal;

namespace SimpQ.Core.Contexts;

/// <summary>
/// Represents a context for managing SQL parameters when building raw queries.
/// Ensures unique parameter naming and tracks values with their corresponding database types.
/// </summary>
public class ParameterContext {
    private readonly List<Parameter> _parameters = [];
    private int _index = 0;

    /// <summary>
    /// Gets the collection of parameters added to the context.
    /// </summary>
    public IReadOnlyCollection<Parameter> Parameters => _parameters.AsReadOnly();

    /// <summary>
    /// Adds a new parameter to the context and returns its generated name.
    /// </summary>
    /// <param name="value">The value to be bound to the parameter.</param>
    /// <param name="dbType">The database type identifier for the parameter (e.g., SQL type code).</param>
    /// <returns>The unique parameter name (e.g., "@p0", "@p1").</returns>
    public string Add(object value, int dbType) {
        var name = $"@p{_index++}";
        _parameters.Add(new Parameter(name, value, dbType));
        return name;
    }
}