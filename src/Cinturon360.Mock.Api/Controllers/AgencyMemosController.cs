using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Api.Data.Entities;
using Cinturon360.Mock.Api.Services;
using Cinturon360.Mock.Shared.Dtos;
using Cinturon360.Mock.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/agency-memos")]
public sealed class AgencyMemosController(AppDbContext db, AuditService audit) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AgencyMemoDto>>> GetAll()
    {
        var memos = await db.AgencyMemos
            .Include(m => m.TmcOrganisation)
            .Include(m => m.ClientOrganisation)
            .Include(m => m.Booking)
            .ToListAsync();
        return Ok(memos.Select(Map));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AgencyMemoDto>> Get(Guid id)
    {
        var m = await db.AgencyMemos
            .Include(m => m.TmcOrganisation)
            .Include(m => m.ClientOrganisation)
            .Include(m => m.Booking)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (m == null) return NotFound();
        return Ok(Map(m));
    }

    [HttpPost("{id:guid}/set-outcome")]
    public async Task<IActionResult> SetOutcome(Guid id, [FromBody] SetMemoOutcomeRequest req, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        var memo = await db.AgencyMemos.Include(m => m.Booking).FirstOrDefaultAsync(m => m.Id == id);
        if (memo == null) return NotFound();

        memo.Outcome = req.Outcome;
        memo.Status = req.Outcome == AgencyMemoOutcome.DisputedWithAirline
            ? AgencyMemoStatus.Disputed
            : AgencyMemoStatus.Settled;

        var ledgerType = req.Outcome switch
        {
            AgencyMemoOutcome.AbsorbedByTmc => LedgerEntryType.AdmAbsorbed,
            AgencyMemoOutcome.RechargedToClient => LedgerEntryType.AdmRecharged,
            AgencyMemoOutcome.DisputedWithAirline => LedgerEntryType.AdmDisputed,
            AgencyMemoOutcome.CreditedToClient => LedgerEntryType.AcmRaised,
            _ => LedgerEntryType.AdmAbsorbed
        };

        var tmc = await db.Organisations.FindAsync(memo.TmcOrganisationId);
        var client = memo.ClientOrganisationId.HasValue ? await db.Organisations.FindAsync(memo.ClientOrganisationId.Value) : tmc;

        db.LedgerEntries.Add(new LedgerEntry
        {
            Id = Guid.NewGuid(),
            SellerOrganisationId = memo.TmcOrganisationId,
            BuyerOrganisationId = memo.ClientOrganisationId ?? memo.TmcOrganisationId,
            BookingId = memo.BookingId,
            EntryType = ledgerType,
            Description = $"{memo.MemoType} {memo.MemoNumber} — {req.Outcome}",
            DebitAmount = req.Outcome == AgencyMemoOutcome.CreditedToClient ? 0 : Math.Abs(memo.Amount),
            CreditAmount = req.Outcome == AgencyMemoOutcome.CreditedToClient ? Math.Abs(memo.Amount) : 0,
            CurrencyCode = memo.CurrencyCode
        });

        // If recharge, add invoice line on existing invoice for this booking
        if (req.Outcome == AgencyMemoOutcome.RechargedToClient && memo.BookingId.HasValue)
        {
            var invoice = await db.InternalInvoices.FirstOrDefaultAsync(i => i.BookingId == memo.BookingId);
            if (invoice != null)
            {
                db.InternalInvoiceLines.Add(new InternalInvoiceLine
                {
                    Id = Guid.NewGuid(),
                    InternalInvoiceId = invoice.Id,
                    LineType = InvoiceLineType.AdmRecharge,
                    Description = $"ADM recharge: {memo.MemoNumber} — {memo.ReasonDescription}",
                    Quantity = 1,
                    UnitAmount = Math.Abs(memo.Amount),
                    TaxAmount = 0,
                    LineTotalAmount = Math.Abs(memo.Amount)
                });
                invoice.TotalAmount += Math.Abs(memo.Amount);
            }
        }

        await db.SaveChangesAsync();
        await audit.LogAsync(actorEmail ?? "system", "AgencyMemo", id, "MemoOutcomeSet", $"Memo {memo.MemoNumber} outcome: {req.Outcome}");
        return Ok();
    }

    [HttpPost("import-csv")]
    public async Task<IActionResult> ImportCsv([FromBody] string csvContent, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        var tmc = await db.Organisations.FirstAsync(o => o.Code == "HELLOWORLD");
        var client = await db.Organisations.FirstAsync(o => o.Code == "COCACOLA");
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        int count = 0;
        foreach (var line in lines.Skip(1))
        {
            var parts = line.Split(',');
            if (parts.Length < 10) continue;
            var memoNum = parts[0].Trim();
            if (await db.AgencyMemos.AnyAsync(m => m.MemoNumber == memoNum)) continue;
            var memoTypeStr = parts[1].Trim().ToUpper();
            var memoType = memoTypeStr == "ACM" ? AgencyMemoType.Acm : AgencyMemoType.Adm;
            var ticketNum = parts[2].Trim();
            var pnr = parts[3].Trim();
            var airline = parts[4].Trim();
            var reason = parts[5].Trim();
            var reasonDesc = parts[6].Trim();
            var amount = decimal.TryParse(parts[7].Trim(), out var a) ? a : 0;
            var currency = parts[8].Trim();

            // Try to match booking by ticket number
            var booking = await db.Bookings.FirstOrDefaultAsync(b => b.TicketNumber == ticketNum || b.PnrCode == pnr);

            var memo = new AgencyMemo
            {
                Id = Guid.NewGuid(),
                MemoType = memoType,
                MemoNumber = memoNum,
                AirlineCode = airline,
                TicketNumber = ticketNum,
                PnrCode = pnr,
                BookingId = booking?.Id,
                ClientOrganisationId = booking != null ? client.Id : null,
                TmcOrganisationId = tmc.Id,
                ReasonCode = reason,
                ReasonDescription = reasonDesc,
                Amount = amount,
                CurrencyCode = currency,
                Status = booking != null ? AgencyMemoStatus.Matched : AgencyMemoStatus.Unmatched
            };
            db.AgencyMemos.Add(memo);

            db.LedgerEntries.Add(new LedgerEntry
            {
                Id = Guid.NewGuid(),
                SellerOrganisationId = tmc.Id,
                BuyerOrganisationId = client.Id,
                BookingId = booking?.Id,
                EntryType = memoType == AgencyMemoType.Adm ? LedgerEntryType.AdmRaised : LedgerEntryType.AcmRaised,
                Description = $"{memoType} {memoNum} imported — {reasonDesc}",
                DebitAmount = memoType == AgencyMemoType.Adm ? Math.Abs(amount) : 0,
                CreditAmount = memoType == AgencyMemoType.Acm ? Math.Abs(amount) : 0,
                CurrencyCode = currency
            });
            count++;
        }
        await db.SaveChangesAsync();
        await audit.LogAsync(actorEmail ?? "system", "AgencyMemo", null, "BspMemoImport", $"Imported {count} ADM/ACM records");
        return Ok(new { imported = count });
    }

    private static AgencyMemoDto Map(AgencyMemo m) => new()
    {
        Id = m.Id,
        MemoType = m.MemoType,
        MemoNumber = m.MemoNumber,
        AirlineCode = m.AirlineCode,
        TicketNumber = m.TicketNumber,
        PnrCode = m.PnrCode,
        BookingReference = m.Booking?.BookingReference,
        ReasonCode = m.ReasonCode,
        ReasonDescription = m.ReasonDescription,
        Amount = m.Amount,
        CurrencyCode = m.CurrencyCode,
        Status = m.Status,
        Outcome = m.Outcome,
        TmcOrganisationName = m.TmcOrganisation?.Name ?? "",
        ClientOrganisationName = m.ClientOrganisation?.Name
    };
}
