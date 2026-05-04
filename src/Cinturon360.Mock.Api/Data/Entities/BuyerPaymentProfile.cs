namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class BuyerPaymentProfile
{
    public Guid Id { get; set; }
    public Guid SellerOrganisationId { get; set; }
    public Organisation? SellerOrganisation { get; set; }
    public Guid BuyerOrganisationId { get; set; }
    public Organisation? BuyerOrganisation { get; set; }
    public Guid PaymentProviderAccountId { get; set; }
    public PaymentProviderAccount? PaymentProviderAccount { get; set; }
    public string FakeProviderCustomerId { get; set; } = string.Empty;
    public List<SavedPaymentMethod> SavedPaymentMethods { get; set; } = [];
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
