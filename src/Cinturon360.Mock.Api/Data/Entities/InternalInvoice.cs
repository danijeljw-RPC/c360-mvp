using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class InternalInvoice
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid SellerOrganisationId { get; set; }
    public Organisation? SellerOrganisation { get; set; }
    public Guid BuyerOrganisationId { get; set; }
    public Organisation? BuyerOrganisation { get; set; }
    public Guid? BookingId { get; set; }
    public Booking? Booking { get; set; }
    public InvoiceStatus Status { get; set; }
    public InvoiceCollectionMode CollectionMode { get; set; }
    public PaymentProviderType PaymentProviderType { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public decimal SubtotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? FakeExternalInvoiceId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<InternalInvoiceLine> Lines { get; set; } = [];
    public List<PaymentAttempt> PaymentAttempts { get; set; } = [];
    public List<LedgerEntry> LedgerEntries { get; set; } = [];
}
