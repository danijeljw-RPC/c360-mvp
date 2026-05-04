namespace Cinturon360.Mock.Shared.Dtos;
public sealed class PaymentMethodDto
{
    public Guid Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public bool IsDefault { get; set; }
    public string DisplayLabel { get; set; } = string.Empty;
    public string BuyerOrganisationName { get; set; } = string.Empty;
}
