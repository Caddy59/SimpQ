namespace SimpQ.UnitTests.Shared.Mocks.Entities;

public class MockEntity : IReportEntity {
    [Column(1)]
    [DefaultOrder]
    public int Id { get; set; }

    [Column(2, name: "FullName")]
    [AllowedToFilter]
    public string Name { get; set; } = default!;

    [Column(3)]
    [AllowedToOrder]
    public int? Age { get; set; }
}