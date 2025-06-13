namespace SimpQ.Abstractions.Models.Requests;

/// <summary>
/// Represents the keyset pagination filter used to fetch the next page of results based on the last retrieved key values.
/// </summary>
/// <param name="Keyset">A dictionary of key column names and their corresponding values from the last retrieved row.</param>
/// <param name="IsDescending">Indicates whether the pagination should be performed in descending order.</param>
public record KeysetFilter(IReadOnlyDictionary<string, object>? Keyset, bool IsDescending);