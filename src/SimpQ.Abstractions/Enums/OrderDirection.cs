namespace SimpQ.Abstractions.Enums;

/// <summary>
/// Specifies the direction in which results should be ordered or sorted.
/// </summary>
public enum OrderDirection : byte {
    /// <summary>
    /// Indicates that the results should be ordered in ascending order (e.g., A to Z, 0 to 9).
    /// </summary>
    Ascending = 0,
    /// <summary>
    /// Indicates that the results should be ordered in descending order (e.g., Z to A, 9 to 0).
    /// </summary>
    Descending = 1
}