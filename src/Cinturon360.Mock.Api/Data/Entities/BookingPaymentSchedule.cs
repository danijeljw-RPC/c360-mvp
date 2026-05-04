using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class BookingPaymentSchedule
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Booking? Booking { get; set; }
    public decimal TotalScheduledAmount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public PaymentScheduleStatus Status { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<BookingPaymentScheduleItem> Items { get; set; } = [];
}
