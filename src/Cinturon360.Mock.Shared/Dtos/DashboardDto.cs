namespace Cinturon360.Mock.Shared.Dtos;
public sealed class DashboardDto
{
    public int BookingsImported { get; set; }
    public int BookingsNeedingReview { get; set; }
    public int InvoicesIssued { get; set; }
    public int PaymentsPending { get; set; }
    public int FailedPayments { get; set; }
    public decimal DepositsAmountDue { get; set; }
    public decimal FutureBalancesAmountDue { get; set; }
    public decimal AdmExposure { get; set; }
    public decimal AcmCredits { get; set; }
    public int ManualInvoices { get; set; }
    public int ExternalSettlementItems { get; set; }
    public decimal ShadowBillingTotal { get; set; }
}
