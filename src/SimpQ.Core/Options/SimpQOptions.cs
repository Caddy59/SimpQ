namespace SimpQ.Core.Options;

/// <summary>
/// Represents configuration options for the SimpQ query engine.
/// Allows consumers to customize behavior such as filter nesting constraints.
/// </summary>
public sealed class SimpQOptions {
    /// <summary>
    /// Gets or sets the maximum allowed nesting level for filter groups.
    /// Used to prevent overly complex or recursive filter conditions.
    /// Default is 2.
    /// </summary>
    public byte MaxFilterNestingLevel { get; set; } = 2;
}