namespace Cinturon360.Mock.Shared.Dtos;
public sealed class GdsImportResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? PnrCode { get; set; }
    public string? BookingReference { get; set; }
    public Guid? BookingId { get; set; }
    public List<string> MissingFields { get; set; } = [];
    public bool NeedsReview { get; set; }
}
