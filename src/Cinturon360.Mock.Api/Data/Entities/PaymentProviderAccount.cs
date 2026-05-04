using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class PaymentProviderAccount
{
    public Guid Id { get; set; }
    public Guid SellerOrganisationId { get; set; }
    public Organisation? SellerOrganisation { get; set; }
    public PaymentProviderType ProviderType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string FakeExternalAccountId { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool IsEnabled { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
