using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class PaymentAttempt
{
    public Guid Id { get; set; }
    public Guid InternalInvoiceId { get; set; }
    public InternalInvoice? InternalInvoice { get; set; }
    public DateTimeOffset AttemptedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public PaymentProviderType ProviderType { get; set; }
    public string? FakeProviderPaymentId { get; set; }
    public Guid? SavedPaymentMethodId { get; set; }
    public SavedPaymentMethod? SavedPaymentMethod { get; set; }
    public bool Succeeded { get; set; }
    public string? FailureReason { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
}
