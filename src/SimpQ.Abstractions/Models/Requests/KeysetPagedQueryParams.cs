namespace SimpQ.Abstractions.Models.Requests;

/// <summary>
/// Represents query parameters for performing keyset-based pagination in a query.
/// Inherits common filtering and ordering options from <see cref="QueryParams"/>.
/// </summary>
public sealed record KeysetPagedQueryParams : QueryParams {
    /// <summary>
    /// Gets or sets the number of items to return per page.
    /// </summary>
    public int PageSize { get; set; } = 25;

    /// <summary>
    /// Gets or sets the key values from the last item of the previous page.
    /// </summary>
    public IReadOnlyDictionary<string, object>? Next { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the pagination should be applied in descending order.
    /// </summary>
    public bool IsDescending { get; set; } = true;
}