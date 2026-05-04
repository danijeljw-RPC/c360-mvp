using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Api.Services;
using Cinturon360.Mock.Shared.Dtos;
using Cinturon360.Mock.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public sealed class BookingsController(AppDbContext db, BillingService billing, AuditService audit) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BookingDto>>> GetAll([FromQuery] string? status, [FromQuery] string? pnr)
    {
        var q = db.Bookings
            .Include(b => b.Traveller)
            .Include(b => b.ClientOrganisation)
            .Include(b => b.TmcOrganisation)
            .Include(b => b.TravelPolicy)
            .Include(b => b.Segments)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<BookingStatus>(status, out var s))
            q = q.Where(b => b.Status == s);
        if (!string.IsNullOrEmpty(pnr))
            q = q.Where(b => b.PnrCode!.Contains(pnr));

        var list = await q.OrderByDescending(b => b.ImportedAtUtc).ToListAsync();
        return Ok(list.Select(Map));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingDto>> Get(Guid id)
    {
        var b = await db.Bookings
            .Include(x => x.Traveller)
            .Include(x => x.ClientOrganisation)
            .Include(x => x.TmcOrganisation)
            .Include(x => x.TravelPolicy).ThenInclude(p => p!.SavedPaymentMethod)
            .Include(x => x.Segments)
            .Include(x => x.PaymentSchedules).ThenInclude(s => s.Items)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return NotFound();
        return Ok(Map(b));
    }

    [HttpPost("{id:guid}/mark-ready-for-billing")]
    public async Task<IActionResult> MarkReady(Guid id, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        var b = await db.Bookings.FindAsync(id);
        if (b == null) return NotFound();
        b.Status = BookingStatus.ReadyForBilling;
        await db.SaveChangesAsync();
        await audit.LogAsync(actorEmail ?? "system", "Booking", id, "MarkedReadyForBilling", $"Booking {b.BookingReference} marked ready for billing");
        return Ok();
    }

    [HttpPost("{id:guid}/generate-invoice")]
    public async Task<ActionResult<InvoiceDto>> GenerateInvoice(Guid id, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        try
        {
            var invoice = await billing.GenerateInvoiceAsync(id, actorEmail ?? "system");
            return Ok(new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                Status = invoice.Status,
                CollectionMode = invoice.CollectionMode,
                PaymentProviderType = invoice.PaymentProviderType,
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                CurrencyCode = invoice.CurrencyCode,
                SubtotalAmount = invoice.SubtotalAmount,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:guid}/set-cost-centre")]
    public async Task<IActionResult> SetCostCentre(Guid id, [FromBody] SetCostCentreRequest req, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        var b = await db.Bookings.FindAsync(id);
        if (b == null) return NotFound();
        b.CostCentreCode = req.CostCentreCode;
        if (b.Status == BookingStatus.NeedsReview && b.TravelPolicyId != null)
            b.Status = BookingStatus.ReadyForBilling;
        await db.SaveChangesAsync();
        await audit.LogAsync(actorEmail ?? "system", "Booking", id, "CostCentreRemediated", $"Cost centre set to {req.CostCentreCode}");
        return Ok();
    }

    [HttpPost("{id:guid}/set-travel-policy")]
    public async Task<IActionResult> SetTravelPolicy(Guid id, [FromBody] SetTravelPolicyRequest req, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        var b = await db.Bookings.FindAsync(id);
        if (b == null) return NotFound();
        b.TravelPolicyId = req.TravelPolicyId;
        if (b.Status == BookingStatus.NeedsReview && !string.IsNullOrEmpty(b.CostCentreCode))
            b.Status = BookingStatus.ReadyForBilling;
        await db.SaveChangesAsync();
        await audit.LogAsync(actorEmail ?? "system", "Booking", id, "TravelPolicyAssigned", $"Travel policy {req.TravelPolicyId} assigned");
        return Ok();
    }

    private static BookingDto Map(Data.Entities.Booking b)
    {
        var missing = new List<string>();
        if (b.TravelPolicyId == null) missing.Add("TravelPolicy");
        if (string.IsNullOrEmpty(b.CostCentreCode)) missing.Add("CostCentreCode");
        if (b.SupplierAmount == 0) missing.Add("SupplierAmount");
        return new BookingDto
        {
            Id = b.Id,
            BookingReference = b.BookingReference,
            PnrCode = b.PnrCode,
            TravellerName = b.Traveller?.FullName ?? "",
            TravellerEmail = b.Traveller?.Email ?? "",
            ClientOrganisationName = b.ClientOrganisation?.Name ?? "",
            TmcOrganisationName = b.TmcOrganisation?.Name ?? "",
            TravelPolicyCode = b.TravelPolicy?.Code,
            TravelPolicyName = b.TravelPolicy?.Name,
            CostCentreCode = b.CostCentreCode,
            Status = b.Status,
            DepartureDate = b.DepartureDate,
            ReturnDate = b.ReturnDate,
            CurrencyCode = b.CurrencyCode,
            SupplierAmount = b.SupplierAmount,
            TaxAmount = b.TaxAmount,
            ServiceFeeAmount = b.ServiceFeeAmount,
            SurchargeAmount = b.SurchargeAmount,
            TotalAmount = b.TotalAmount,
            SourceSystem = b.SourceSystem,
            MissingFields = missing,
            Segments = b.Segments.Select(s => new BookingSegmentDto
            {
                Id = s.Id,
                SegmentType = s.SegmentType,
                Carrier = s.Carrier,
                FlightNumber = s.FlightNumber,
                FromLocation = s.FromLocation,
                ToLocation = s.ToLocation,
                DepartureUtc = s.DepartureUtc,
                ArrivalUtc = s.ArrivalUtc
            }).ToList()
        };
    }
}
