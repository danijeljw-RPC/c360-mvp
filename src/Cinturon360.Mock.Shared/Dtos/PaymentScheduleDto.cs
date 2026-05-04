using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Shared.Dtos;
public sealed class PaymentScheduleDto
{
    public Guid Id { get; set; }
    public decimal TotalScheduledAmount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public PaymentScheduleStatus Status { get; set; }
    public List<PaymentScheduleItemDto> Items { get; set; } = [];
}
public sealed class PaymentScheduleItemDto
{
    public Guid Id { get; set; }
    public PaymentScheduleItemType ItemType { get; set; }
    public PaymentTiming PaymentTiming { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly? DueDate { get; set; }
    public PaymentScheduleItemStatus Status { get; set; }
}
