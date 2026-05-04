using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Shared.Dtos;
public sealed class AgencyMemoDto
{
    public Guid Id { get; set; }
    public AgencyMemoType MemoType { get; set; }
    public string MemoNumber { get; set; } = string.Empty;
    public string AirlineCode { get; set; } = string.Empty;
    public string? TicketNumber { get; set; }
    public string? PnrCode { get; set; }
    public string? BookingReference { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonDescription { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public AgencyMemoStatus Status { get; set; }
    public AgencyMemoOutcome? Outcome { get; set; }
    public string TmcOrganisationName { get; set; } = string.Empty;
    public string? ClientOrganisationName { get; set; }
}
