namespace SimpQ.Abstractions.Models.Requests;

/// <summary>
/// Represents query parameters for performing offset-based pagination in a dynamic query.
/// Inherits common filtering and ordering options from <see cref="QueryParams"/>.
/// </summary>
public sealed record OffsetPagedQueryParams : QueryParams {
    /// <summary>
    /// Gets or sets the current page number (1-based index).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items to return per page.
    /// </summary>
    public int PageSize { get; set; } = 25;
}