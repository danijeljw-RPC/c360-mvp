using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Shared.Dtos;
using Cinturon360.Mock.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController(AppDbContext db) : ControllerBase
{
    [HttpGet("outstanding-balances")]
    public async Task<ActionResult<List<OutstandingBalanceReportItem>>> OutstandingBalances()
    {
        var items = await db.InternalInvoices
            .Include(i => i.Booking).ThenInclude(b => b!.Traveller)
            .Include(i => i.BuyerOrganisation)
            .Where(i => i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.PaymentPending || i.Status == InvoiceStatus.Failed || i.Status == InvoiceStatus.PartiallyPaid)
            .ToListAsync();
        return Ok(items.Select(i => new OutstandingBalanceReportItem
        {
            BookingReference = i.Booking?.BookingReference ?? "-",
            TravellerName = i.Booking?.Traveller?.FullName ?? "-",
            ClientOrganisationName = i.BuyerOrganisation?.Name ?? "-",
            InvoiceNumber = i.InvoiceNumber,
            Status = i.Status.ToString(),
            AmountDue = i.TotalAmount,
            CurrencyCode = i.CurrencyCode,
            DueDate = i.DueDate
        }));
    }

    [HttpGet("future-balances")]
    public async Task<ActionResult> FutureBalances()
    {
        var items = await db.BookingPaymentScheduleItems
            .Include(i => i.BookingPaymentSchedule).ThenInclude(s => s.Booking).ThenInclude(b => b!.Traveller)
            .Include(i => i.BookingPaymentSchedule).ThenInclude(s => s.Booking).ThenInclude(b => b!.ClientOrganisation)
            .Where(i => i.PaymentTiming == PaymentTiming.DueOnDate && i.Status == PaymentScheduleItemStatus.Pending)
            .ToListAsync();
        return Ok(items.Select(i => new
        {
            BookingReference = i.BookingPaymentSchedule?.Booking?.BookingReference,
            TravellerName = i.BookingPaymentSchedule?.Booking?.Traveller?.FullName,
            ClientOrganisationName = i.BookingPaymentSchedule?.Booking?.ClientOrganisation?.Name,
            i.Description,
            i.Amount,
            CurrencyCode = i.BookingPaymentSchedule?.CurrencyCode ?? "AUD",
            i.DueDate
        }));
    }

    [HttpGet("failed-payments")]
    public async Task<ActionResult> FailedPayments()
    {
        var items = await db.InternalInvoices
            .Include(i => i.Booking)
            .Include(i => i.BuyerOrganisation)
            .Where(i => i.Status == InvoiceStatus.Failed)
            .ToListAsync();
        return Ok(items.Select(i => new
        {
            i.InvoiceNumber,
            BookingReference = i.Booking?.BookingReference,
            BuyerOrg = i.BuyerOrganisation?.Name,
            i.TotalAmount,
            i.CurrencyCode
        }));
    }

    [HttpGet("service-fees")]
    public async Task<ActionResult<List<ServiceFeeReportItem>>> ServiceFees()
    {
        var lines = await db.InternalInvoiceLines
            .Include(l => l.InternalInvoice).ThenInclude(i => i!.Booking).ThenInclude(b => b!.Traveller)
            .Where(l => l.LineType == InvoiceLineType.BookingFee || l.LineType == InvoiceLineType.AmendmentFee || l.LineType == InvoiceLineType.AfterHoursFee || l.LineType == InvoiceLineType.PolicyExceptionFee)
            .ToListAsync();
        return Ok(lines.Select(l => new ServiceFeeReportItem
        {
            BookingReference = l.InternalInvoice?.Booking?.BookingReference ?? "-",
            TravellerName = l.InternalInvoice?.Booking?.Traveller?.FullName ?? "-",
            FeeDescription = l.Description,
            Amount = l.LineTotalAmount,
            CurrencyCode = l.InternalInvoice?.CurrencyCode ?? "AUD"
        }));
    }

    [HttpGet("adm-acm-exposure")]
    public async Task<ActionResult<List<AdmAcmExposureItem>>> AdmAcmExposure()
    {
        var memos = await db.AgencyMemos.Include(m => m.Booking).ToListAsync();
        return Ok(memos.Select(m => new AdmAcmExposureItem
        {
            MemoNumber = m.MemoNumber,
            MemoType = m.MemoType.ToString(),
            AirlineCode = m.AirlineCode,
            TicketNumber = m.TicketNumber,
            BookingReference = m.Booking?.BookingReference,
            ReasonCode = m.ReasonCode,
            Amount = m.Amount,
            CurrencyCode = m.CurrencyCode,
            Status = m.Status.ToString()
        }));
    }

    [HttpGet("shadow-billing")]
    public async Task<ActionResult> ShadowBilling()
    {
        var items = await db.InternalInvoices
            .Include(i => i.Booking)
            .Include(i => i.BuyerOrganisation)
            .Where(i => i.CollectionMode == InvoiceCollectionMode.ShadowOnly)
            .ToListAsync();
        return Ok(items.Select(i => new
        {
            i.InvoiceNumber,
            BookingReference = i.Booking?.BookingReference,
            BuyerOrg = i.BuyerOrganisation?.Name,
            i.TotalAmount,
            i.CurrencyCode,
            i.Status
        }));
    }

    [HttpGet("supplier-reconciliation")]
    public async Task<ActionResult> SupplierReconciliation()
    {
        var bookings = await db.Bookings
            .Include(b => b.Traveller)
            .Include(b => b.TravelPolicy)
            .Include(b => b.Invoices)
            .ToListAsync();
        return Ok(bookings.Select(b => new
        {
            b.BookingReference,
            b.PnrCode,
            b.TicketNumber,
            Traveller = b.Traveller?.FullName,
            Policy = b.TravelPolicy?.Code,
            b.SupplierAmount,
            b.TaxAmount,
            b.TotalAmount,
            InvoiceStatus = b.Invoices.FirstOrDefault()?.Status.ToString() ?? "NotInvoiced"
        }));
    }
}
