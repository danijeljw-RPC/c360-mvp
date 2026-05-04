namespace Cinturon360.Mock.Shared.Dtos;
public sealed class BookingSegmentDto
{
    public Guid Id { get; set; }
    public string SegmentType { get; set; } = string.Empty;
    public string? Carrier { get; set; }
    public string? FlightNumber { get; set; }
    public string? FromLocation { get; set; }
    public string? ToLocation { get; set; }
    public DateTimeOffset? DepartureUtc { get; set; }
    public DateTimeOffset? ArrivalUtc { get; set; }
    public string? SupplierName { get; set; }
}
