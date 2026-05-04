using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Shared.Dtos;
public sealed class BillingRelationshipDto
{
    public Guid Id { get; set; }
    public Guid SellerOrganisationId { get; set; }
    public string SellerOrganisationName { get; set; } = string.Empty;
    public Guid BuyerOrganisationId { get; set; }
    public string BuyerOrganisationName { get; set; } = string.Empty;
    public BillingMode BillingMode { get; set; }
    public PaymentProviderType PaymentProviderType { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public int PaymentTermsDays { get; set; }
    public bool IsCommercialRiskHeldBySeller { get; set; }
}
