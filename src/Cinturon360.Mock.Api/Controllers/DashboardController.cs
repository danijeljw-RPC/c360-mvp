using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Shared.Dtos;
using Cinturon360.Mock.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        var bookings = await db.Bookings.ToListAsync();
        var invoices = await db.InternalInvoices.ToListAsync();
        var memos = await db.AgencyMemos.ToListAsync();
        var scheduleItems = await db.BookingPaymentScheduleItems.ToListAsync();

        return Ok(new DashboardDto
        {
            BookingsImported = bookings.Count,
            BookingsNeedingReview = bookings.Count(b => b.Status == BookingStatus.NeedsReview),
            InvoicesIssued = invoices.Count(i => i.Status != InvoiceStatus.Draft),
            PaymentsPending = invoices.Count(i => i.Status == InvoiceStatus.PaymentPending || i.Status == InvoiceStatus.Issued),
            FailedPayments = invoices.Count(i => i.Status == InvoiceStatus.Failed),
            DepositsAmountDue = scheduleItems.Where(x => x.ItemType == PaymentScheduleItemType.Deposit && x.Status == PaymentScheduleItemStatus.Pending).Sum(x => x.Amount),
            FutureBalancesAmountDue = scheduleItems.Where(x => x.ItemType == PaymentScheduleItemType.SupplierBalance && x.PaymentTiming == PaymentTiming.DueOnDate && x.Status == PaymentScheduleItemStatus.Pending).Sum(x => x.Amount),
            AdmExposure = memos.Where(x => x.MemoType == AgencyMemoType.Adm && x.Status != AgencyMemoStatus.Settled && x.Status != AgencyMemoStatus.Closed).Sum(x => x.Amount),
            AcmCredits = memos.Where(x => x.MemoType == AgencyMemoType.Acm).Sum(x => Math.Abs(x.Amount)),
            ManualInvoices = invoices.Count(i => i.CollectionMode == InvoiceCollectionMode.ManualCollection),
            ExternalSettlementItems = invoices.Count(i => i.CollectionMode == InvoiceCollectionMode.ExternalFinanceSystem),
            ShadowBillingTotal = invoices.Where(i => i.CollectionMode == InvoiceCollectionMode.ShadowOnly).Sum(i => i.TotalAmount)
        });
    }
}
