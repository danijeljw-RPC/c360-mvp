using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Shared.Dtos;
public sealed class TravelPolicyDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TravelPolicyBillingBehaviour BillingBehaviour { get; set; }
    public Guid? SavedPaymentMethodId { get; set; }
    public string? PaymentMethodLabel { get; set; }
    public string ClientOrganisationName { get; set; } = string.Empty;
}
