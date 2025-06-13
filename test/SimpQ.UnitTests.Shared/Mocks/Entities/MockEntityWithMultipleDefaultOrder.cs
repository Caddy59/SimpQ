namespace SimpQ.UnitTests.Shared.Mocks.Entities;

public class MockEntityWithMultipleDefaultOrder : IReportEntity {
    [Column(1)]
    [DefaultOrder]
    public int Id { get; set; }

    [Column(2)]
    [DefaultOrder(1, OrderDirection.Descending)]
    public string Name { get; set; } = default!;
}