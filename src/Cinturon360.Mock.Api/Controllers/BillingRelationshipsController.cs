using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/billing-relationships")]
public sealed class BillingRelationshipsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BillingRelationshipDto>>> GetAll()
    {
        var rels = await db.BillingRelationships
            .Include(r => r.SellerOrganisation)
            .Include(r => r.BuyerOrganisation)
            .ToListAsync();
        return Ok(rels.Select(r => new BillingRelationshipDto
        {
            Id = r.Id,
            SellerOrganisationId = r.SellerOrganisationId,
            SellerOrganisationName = r.SellerOrganisation!.Name,
            BuyerOrganisationId = r.BuyerOrganisationId,
            BuyerOrganisationName = r.BuyerOrganisation!.Name,
            BillingMode = r.BillingMode,
            PaymentProviderType = r.PaymentProviderType,
            CurrencyCode = r.CurrencyCode,
            PaymentTermsDays = r.PaymentTermsDays,
            IsCommercialRiskHeldBySeller = r.IsCommercialRiskHeldBySeller
        }));
    }
}
