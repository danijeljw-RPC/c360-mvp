using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class AgencyMemo
{
    public Guid Id { get; set; }
    public AgencyMemoType MemoType { get; set; }
    public string MemoNumber { get; set; } = string.Empty;
    public string AirlineCode { get; set; } = string.Empty;
    public string? TicketNumber { get; set; }
    public string? EmdNumber { get; set; }
    public string? PnrCode { get; set; }
    public Guid? BookingId { get; set; }
    public Booking? Booking { get; set; }
    public Guid TmcOrganisationId { get; set; }
    public Organisation? TmcOrganisation { get; set; }
    public Guid? ClientOrganisationId { get; set; }
    public Organisation? ClientOrganisation { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonDescription { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public AgencyMemoStatus Status { get; set; }
    public AgencyMemoOutcome? Outcome { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
