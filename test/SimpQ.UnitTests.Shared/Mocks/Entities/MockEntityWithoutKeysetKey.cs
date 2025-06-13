namespace SimpQ.UnitTests.Shared.Mocks.Entities;

public class MockEntityWithoutKeysetKey : IReportEntity {
    [Column(1)]
    [DefaultOrder]
    public int Id { get; set; }

    [Column(2)]
    [AllowedToFilter]
    public string Name { get; set; } = default!;
}