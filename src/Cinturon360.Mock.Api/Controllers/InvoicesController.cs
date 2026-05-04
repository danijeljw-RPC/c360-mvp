using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Api.Services;
using Cinturon360.Mock.Shared.Dtos;
using Cinturon360.Mock.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController(AppDbContext db, PaymentSimulationService paymentSim, AuditService audit) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<InvoiceDto>>> GetAll()
    {
        var list = await db.InternalInvoices
            .Include(i => i.SellerOrganisation)
            .Include(i => i.BuyerOrganisation)
            .Include(i => i.Booking)
            .Include(i => i.Lines)
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync();
        return Ok(list.Select(Map));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvoiceDto>> Get(Guid id)
    {
        var inv = await db.InternalInvoices
            .Include(i => i.SellerOrganisation)
            .Include(i => i.BuyerOrganisation)
            .Include(i => i.Booking)
            .Include(i => i.Lines)
            .Include(i => i.PaymentAttempts).ThenInclude(a => a.SavedPaymentMethod)
            .Include(i => i.LedgerEntries).ThenInclude(l => l.SellerOrganisation)
            .Include(i => i.LedgerEntries).ThenInclude(l => l.BuyerOrganisation)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (inv == null) return NotFound();
        return Ok(Map(inv));
    }

    [HttpPost("{id:guid}/simulate-collection")]
    public async Task<ActionResult> SimulateCollection(Guid id, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        var (succeeded, message) = await paymentSim.SimulateAsync(id, actorEmail ?? "system");
        return Ok(new { succeeded, message, note = "Provider action simulated only. No external payment was made. Cinturon360 internal invoice remains the source of truth." });
    }

    [HttpPost("{id:guid}/retry-payment")]
    public async Task<ActionResult> RetryPayment(Guid id, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        var inv = await db.InternalInvoices.FindAsync(id);
        if (inv == null) return NotFound();
        if (inv.Status != InvoiceStatus.Failed) return BadRequest("Invoice is not in failed state");
        var (succeeded, message) = await paymentSim.SimulateAsync(id, actorEmail ?? "system");
        return Ok(new { succeeded, message });
    }

    [HttpPost("{id:guid}/mark-manual-payment-received")]
    public async Task<ActionResult> MarkManualPaymentReceived(Guid id, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        var inv = await db.InternalInvoices.FindAsync(id);
        if (inv == null) return NotFound();
        inv.Status = InvoiceStatus.Paid;
        db.LedgerEntries.Add(new Data.Entities.LedgerEntry
        {
            Id = Guid.NewGuid(),
            SellerOrganisationId = inv.SellerOrganisationId,
            BuyerOrganisationId = inv.BuyerOrganisationId,
            BookingId = inv.BookingId,
            InternalInvoiceId = id,
            EntryType = LedgerEntryType.ManualPaymentRecorded,
            Description = $"Manual payment recorded for invoice {inv.InvoiceNumber}",
            DebitAmount = 0,
            CreditAmount = inv.TotalAmount,
            CurrencyCode = inv.CurrencyCode
        });
        await db.SaveChangesAsync();
        await audit.LogAsync(actorEmail ?? "system", "Invoice", id, "ManualPaymentRecorded", $"Manual payment recorded for {inv.InvoiceNumber}");
        return Ok();
    }

    [HttpPost("{id:guid}/mark-externally-settled")]
    public async Task<ActionResult> MarkExternallySettled(Guid id, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        var inv = await db.InternalInvoices.FindAsync(id);
        if (inv == null) return NotFound();
        inv.Status = InvoiceStatus.Paid;
        db.LedgerEntries.Add(new Data.Entities.LedgerEntry
        {
            Id = Guid.NewGuid(),
            SellerOrganisationId = inv.SellerOrganisationId,
            BuyerOrganisationId = inv.BuyerOrganisationId,
            BookingId = inv.BookingId,
            InternalInvoiceId = id,
            EntryType = LedgerEntryType.ExternalSettlementMarked,
            Description = $"External settlement confirmed for invoice {inv.InvoiceNumber}",
            DebitAmount = 0,
            CreditAmount = inv.TotalAmount,
            CurrencyCode = inv.CurrencyCode
        });
        await db.SaveChangesAsync();
        await audit.LogAsync(actorEmail ?? "system", "Invoice", id, "ExternalSettlementMarked", $"External settlement marked for {inv.InvoiceNumber}");
        return Ok();
    }

    private static InvoiceDto Map(Data.Entities.InternalInvoice i) => new()
    {
        Id = i.Id,
        InvoiceNumber = i.InvoiceNumber,
        SellerOrganisationName = i.SellerOrganisation?.Name ?? "",
        BuyerOrganisationName = i.BuyerOrganisation?.Name ?? "",
        BookingReference = i.Booking?.BookingReference,
        Status = i.Status,
        CollectionMode = i.CollectionMode,
        PaymentProviderType = i.PaymentProviderType,
        IssueDate = i.IssueDate,
        DueDate = i.DueDate,
        CurrencyCode = i.CurrencyCode,
        SubtotalAmount = i.SubtotalAmount,
        TaxAmount = i.TaxAmount,
        TotalAmount = i.TotalAmount,
        FakeExternalInvoiceId = i.FakeExternalInvoiceId,
        Lines = i.Lines.Select(l => new InvoiceLineDto
        {
            Id = l.Id,
            LineType = l.LineType,
            Description = l.Description,
            Quantity = l.Quantity,
            UnitAmount = l.UnitAmount,
            TaxAmount = l.TaxAmount,
            LineTotalAmount = l.LineTotalAmount
        }).ToList()
    };
}
