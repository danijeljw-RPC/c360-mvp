using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Shared.Dtos;
public sealed class LedgerEntryDto
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public LedgerEntryType EntryType { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public string? ExternalReference { get; set; }
    public string SellerOrganisationName { get; set; } = string.Empty;
    public string BuyerOrganisationName { get; set; } = string.Empty;
}
