using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class TravelPolicy
{
    public Guid Id { get; set; }
    public Guid ClientOrganisationId { get; set; }
    public Organisation? ClientOrganisation { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TravelPolicyBillingBehaviour BillingBehaviour { get; set; }
    public Guid? SavedPaymentMethodId { get; set; }
    public SavedPaymentMethod? SavedPaymentMethod { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
