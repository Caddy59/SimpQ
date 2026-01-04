namespace SimpQ.UnitTests.Shared.Mocks.Entities;

/// <summary>
/// Mock entity configured entirely through fluent API without attributes.
/// </summary>
public class MockEntityFluentOnly : IReportEntity {
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int? Age { get; set; }
    public DateTime CreatedDate { get; set; }
}
