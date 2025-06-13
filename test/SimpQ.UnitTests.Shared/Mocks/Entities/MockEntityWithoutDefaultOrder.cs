namespace SimpQ.UnitTests.Shared.Mocks.Entities;

public class MockEntityWithoutDefaultOrder : IReportEntity {
    [Column(1)]
    public int Id { get; set; }

    [Column(2)]
    public string Name { get; set; } = default!;
}