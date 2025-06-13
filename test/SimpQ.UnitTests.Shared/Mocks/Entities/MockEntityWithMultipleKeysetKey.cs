namespace SimpQ.UnitTests.Shared.Mocks.Entities;

public class MockEntityWithMultipleKeysetKey : IReportEntity {
    [Column(1)]
    [KeysetPaginationKey]
    public int Id { get; set; }

    [Column(2)]
    [KeysetPaginationKey(1)]
    public string Name { get; set; } = default!;
}