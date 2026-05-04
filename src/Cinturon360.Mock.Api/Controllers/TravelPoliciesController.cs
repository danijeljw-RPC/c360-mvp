using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/travel-policies")]
public sealed class TravelPoliciesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TravelPolicyDto>>> GetAll()
    {
        var policies = await db.TravelPolicies
            .Include(p => p.ClientOrganisation)
            .Include(p => p.SavedPaymentMethod)
            .ToListAsync();
        return Ok(policies.Select(p => new TravelPolicyDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            BillingBehaviour = p.BillingBehaviour,
            SavedPaymentMethodId = p.SavedPaymentMethodId,
            PaymentMethodLabel = p.SavedPaymentMethod != null
                ? $"{p.SavedPaymentMethod.Brand} ending {p.SavedPaymentMethod.Last4}"
                : null,
            ClientOrganisationName = p.ClientOrganisation!.Name
        }));
    }
}
