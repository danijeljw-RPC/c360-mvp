using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/ledger")]
public sealed class LedgerController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<LedgerEntryDto>>> GetAll([FromQuery] Guid? bookingId, [FromQuery] Guid? invoiceId)
    {
        var q = db.LedgerEntries
            .Include(l => l.SellerOrganisation)
            .Include(l => l.BuyerOrganisation)
            .AsQueryable();
        if (bookingId.HasValue) q = q.Where(l => l.BookingId == bookingId);
        if (invoiceId.HasValue) q = q.Where(l => l.InternalInvoiceId == invoiceId);
        var list = await q.OrderByDescending(l => l.CreatedAtUtc).ToListAsync();
        return Ok(list.Select(l => new LedgerEntryDto
        {
            Id = l.Id,
            CreatedAtUtc = l.CreatedAtUtc,
            EntryType = l.EntryType,
            Description = l.Description,
            DebitAmount = l.DebitAmount,
            CreditAmount = l.CreditAmount,
            CurrencyCode = l.CurrencyCode,
            ExternalReference = l.ExternalReference,
            SellerOrganisationName = l.SellerOrganisation?.Name ?? "",
            BuyerOrganisationName = l.BuyerOrganisation?.Name ?? ""
        }));
    }
}
