namespace SimpQ.UnitTests.Shared.Mocks.Entities;

public class MockEntityWithConflictingKeysetKey : IReportEntity {
    [Column(1)]
    [KeysetPaginationKey]
    public int Id { get; set; }

    [Column(2)]
    [KeysetPaginationKey]
    public string Name { get; set; } = default!;
}