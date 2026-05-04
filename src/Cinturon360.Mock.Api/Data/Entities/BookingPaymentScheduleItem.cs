using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class BookingPaymentScheduleItem
{
    public Guid Id { get; set; }
    public Guid BookingPaymentScheduleId { get; set; }
    public BookingPaymentSchedule? BookingPaymentSchedule { get; set; }
    public PaymentScheduleItemType ItemType { get; set; }
    public PaymentTiming PaymentTiming { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly? DueDate { get; set; }
    public PaymentScheduleItemStatus Status { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
