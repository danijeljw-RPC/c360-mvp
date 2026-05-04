using System.Text;
using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Api.Data.Entities;
using Cinturon360.Mock.Api.Services;
using Cinturon360.Mock.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/finance-exports")]
public sealed class FinanceExportsController(AppDbContext db, AuditService audit) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<FinanceExportBatchDto>>> GetAll()
    {
        var batches = await db.FinanceExportBatches.OrderByDescending(b => b.CreatedAtUtc).ToListAsync();
        return Ok(batches.Select(b => new FinanceExportBatchDto
        {
            Id = b.Id,
            ExportTarget = b.ExportTarget,
            CreatedAtUtc = b.CreatedAtUtc,
            InvoiceCount = b.InvoiceCount,
            Status = b.Status
        }));
    }

    [HttpPost]
    public async Task<ActionResult<FinanceExportBatchDto>> Create([FromBody] FinanceExportRequestDto req, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        var invoiceIds = req.InvoiceIds.Count > 0
            ? req.InvoiceIds
            : await db.InternalInvoices.Select(i => i.Id).ToListAsync();

        var batch = new FinanceExportBatch
        {
            Id = Guid.NewGuid(),
            ExportTarget = req.ExportTarget,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            CreatedByEmail = actorEmail ?? "system",
            InvoiceCount = invoiceIds.Count,
            Status = "Completed"
        };

        foreach (var iid in invoiceIds)
            batch.Items.Add(new FinanceExportItem { Id = Guid.NewGuid(), FinanceExportBatchId = batch.Id, InternalInvoiceId = iid });

        db.FinanceExportBatches.Add(batch);
        await db.SaveChangesAsync();
        await audit.LogAsync(actorEmail ?? "system", "FinanceExportBatch", batch.Id, "ExportCreated", $"Finance export to {req.ExportTarget} created with {invoiceIds.Count} invoices");

        return Ok(new FinanceExportBatchDto { Id = batch.Id, ExportTarget = batch.ExportTarget, CreatedAtUtc = batch.CreatedAtUtc, InvoiceCount = batch.InvoiceCount, Status = batch.Status });
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var batch = await db.FinanceExportBatches
            .Include(b => b.Items).ThenInclude(i => i.InternalInvoice).ThenInclude(inv => inv!.Lines)
            .Include(b => b.Items).ThenInclude(i => i.InternalInvoice).ThenInclude(inv => inv!.SellerOrganisation)
            .Include(b => b.Items).ThenInclude(i => i.InternalInvoice).ThenInclude(inv => inv!.BuyerOrganisation)
            .Include(b => b.Items).ThenInclude(i => i.InternalInvoice).ThenInclude(inv => inv!.Booking).ThenInclude(b => b!.Traveller)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (batch == null) return NotFound();

        var sb = new StringBuilder();
        sb.AppendLine("ExportBatchId,ExportTarget,SellerOrganisation,BuyerOrganisation,InvoiceNumber,InvoiceDate,DueDate,BookingReference,LineType,LineDescription,DebitAmount,CreditAmount,TaxAmount,CurrencyCode,Status");

        foreach (var item in batch.Items)
        {
            var inv = item.InternalInvoice;
            if (inv == null) continue;
            foreach (var line in inv.Lines)
            {
                sb.AppendLine($"{batch.Id},{batch.ExportTarget},{inv.SellerOrganisation?.Name},{inv.BuyerOrganisation?.Name},{inv.InvoiceNumber},{inv.IssueDate},{inv.DueDate},{inv.Booking?.BookingReference},{line.LineType},{line.Description},{line.LineTotalAmount},0,{line.TaxAmount},{inv.CurrencyCode},{inv.Status}");
            }
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", $"export-{batch.ExportTarget.ToLower()}-{id}.csv");
    }
}
