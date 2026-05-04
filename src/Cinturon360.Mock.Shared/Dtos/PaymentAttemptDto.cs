namespace Cinturon360.Mock.Shared.Dtos;
public sealed class PaymentAttemptDto
{
    public Guid Id { get; set; }
    public DateTimeOffset AttemptedAtUtc { get; set; }
    public string ProviderType { get; set; } = string.Empty;
    public string? FakeProviderPaymentId { get; set; }
    public string? FakePaymentMethodLabel { get; set; }
    public bool Succeeded { get; set; }
    public string? FailureReason { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public string SimulationNote { get; set; } = "Provider action simulated only. No external payment was made. Cinturon360 internal invoice remains the source of truth.";
}
