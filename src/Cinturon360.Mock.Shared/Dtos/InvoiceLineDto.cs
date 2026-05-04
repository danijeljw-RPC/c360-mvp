using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Shared.Dtos;
public sealed class InvoiceLineDto
{
    public Guid Id { get; set; }
    public InvoiceLineType LineType { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotalAmount { get; set; }
}
