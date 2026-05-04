using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class BillingRelationship
{
    public Guid Id { get; set; }
    public Guid SellerOrganisationId { get; set; }
    public Organisation? SellerOrganisation { get; set; }
    public Guid BuyerOrganisationId { get; set; }
    public Organisation? BuyerOrganisation { get; set; }
    public BillingMode BillingMode { get; set; }
    public PaymentProviderType PaymentProviderType { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public int PaymentTermsDays { get; set; }
    public bool IsCommercialRiskHeldBySeller { get; set; } = true;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
