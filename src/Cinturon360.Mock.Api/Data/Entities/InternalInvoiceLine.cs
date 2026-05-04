using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class InternalInvoiceLine
{
    public Guid Id { get; set; }
    public Guid InternalInvoiceId { get; set; }
    public InternalInvoice? InternalInvoice { get; set; }
    public InvoiceLineType LineType { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotalAmount { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
