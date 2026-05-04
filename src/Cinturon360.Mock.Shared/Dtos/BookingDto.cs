using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Shared.Dtos;
public sealed class BookingDto
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public string? PnrCode { get; set; }
    public string TravellerName { get; set; } = string.Empty;
    public string TravellerEmail { get; set; } = string.Empty;
    public string ClientOrganisationName { get; set; } = string.Empty;
    public string TmcOrganisationName { get; set; } = string.Empty;
    public string? TravelPolicyCode { get; set; }
    public string? TravelPolicyName { get; set; }
    public string? CostCentreCode { get; set; }
    public BookingStatus Status { get; set; }
    public DateOnly DepartureDate { get; set; }
    public DateOnly ReturnDate { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public decimal SupplierAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ServiceFeeAmount { get; set; }
    public decimal SurchargeAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string SourceSystem { get; set; } = string.Empty;
    public List<string> MissingFields { get; set; } = [];
    public List<BookingSegmentDto> Segments { get; set; } = [];
}
