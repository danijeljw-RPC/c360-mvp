using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class LedgerEntry
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public Guid SellerOrganisationId { get; set; }
    public Organisation? SellerOrganisation { get; set; }
    public Guid BuyerOrganisationId { get; set; }
    public Organisation? BuyerOrganisation { get; set; }
    public Guid? BookingId { get; set; }
    public Booking? Booking { get; set; }
    public Guid? InternalInvoiceId { get; set; }
    public InternalInvoice? InternalInvoice { get; set; }
    public LedgerEntryType EntryType { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public string? ExternalReference { get; set; }
}
