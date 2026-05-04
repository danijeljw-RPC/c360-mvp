namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class SavedPaymentMethod
{
    public Guid Id { get; set; }
    public Guid BuyerPaymentProfileId { get; set; }
    public BuyerPaymentProfile? BuyerPaymentProfile { get; set; }
    public string FakeProviderPaymentMethodId { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public bool IsDefault { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
