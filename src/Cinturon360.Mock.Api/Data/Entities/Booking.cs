using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class Booking
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public string? PnrCode { get; set; }
    public string? TicketNumber { get; set; }
    public Guid TravellerId { get; set; }
    public Traveller? Traveller { get; set; }
    public Guid ClientOrganisationId { get; set; }
    public Organisation? ClientOrganisation { get; set; }
    public Guid TmcOrganisationId { get; set; }
    public Organisation? TmcOrganisation { get; set; }
    public Guid VendorOrganisationId { get; set; }
    public Organisation? VendorOrganisation { get; set; }
    public Guid? TravelPolicyId { get; set; }
    public TravelPolicy? TravelPolicy { get; set; }
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
    public DateTimeOffset ImportedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<BookingSegment> Segments { get; set; } = [];
    public List<BookingPaymentSchedule> PaymentSchedules { get; set; } = [];
    public List<InternalInvoice> Invoices { get; set; } = [];
    public List<LedgerEntry> LedgerEntries { get; set; } = [];
    public List<AgencyMemo> AgencyMemos { get; set; } = [];
}
