using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Shared.Dtos;
public sealed class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string SellerOrganisationName { get; set; } = string.Empty;
    public string BuyerOrganisationName { get; set; } = string.Empty;
    public string? BookingReference { get; set; }
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
    public List<InvoiceLineDto> Lines { get; set; } = [];
}
