namespace SimpQ.UnitTests.Shared.Mocks.Entities;

public class MockEntityWithConflictingDefaultOrder : IReportEntity {
    [Column(1)]
    [DefaultOrder]
    public int Id { get; set; }

    [Column(2)]
    [DefaultOrder]
    public string Name { get; set; } = default!;
}