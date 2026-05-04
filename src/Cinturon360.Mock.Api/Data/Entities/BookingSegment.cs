namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class BookingSegment
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Booking? Booking { get; set; }
    public string SegmentType { get; set; } = string.Empty;
    public string? Carrier { get; set; }
    public string? FlightNumber { get; set; }
    public string? FromLocation { get; set; }
    public string? ToLocation { get; set; }
    public DateTimeOffset? DepartureUtc { get; set; }
    public DateTimeOffset? ArrivalUtc { get; set; }
    public string? SupplierName { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
