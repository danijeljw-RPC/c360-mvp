namespace Cinturon360.Mock.Shared.Dtos;
public sealed class OutstandingBalanceReportItem
{
    public string BookingReference { get; set; } = string.Empty;
    public string TravellerName { get; set; } = string.Empty;
    public string ClientOrganisationName { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal AmountDue { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public DateOnly? DueDate { get; set; }
}
public sealed class ServiceFeeReportItem
{
    public string BookingReference { get; set; } = string.Empty;
    public string TravellerName { get; set; } = string.Empty;
    public string FeeDescription { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
}
public sealed class AdmAcmExposureItem
{
    public string MemoNumber { get; set; } = string.Empty;
    public string MemoType { get; set; } = string.Empty;
    public string AirlineCode { get; set; } = string.Empty;
    public string? TicketNumber { get; set; }
    public string? BookingReference { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public string Status { get; set; } = string.Empty;
}
