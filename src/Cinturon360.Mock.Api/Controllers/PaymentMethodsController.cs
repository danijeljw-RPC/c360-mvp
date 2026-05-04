using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/payment-methods")]
public sealed class PaymentMethodsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PaymentMethodDto>>> GetAll()
    {
        var methods = await db.SavedPaymentMethods
            .Include(m => m.BuyerPaymentProfile).ThenInclude(p => p!.BuyerOrganisation)
            .ToListAsync();
        return Ok(methods.Select(m => new PaymentMethodDto
        {
            Id = m.Id,
            Brand = m.Brand,
            Last4 = m.Last4,
            ExpiryMonth = m.ExpiryMonth,
            ExpiryYear = m.ExpiryYear,
            IsDefault = m.IsDefault,
            DisplayLabel = $"{m.Brand} ending {m.Last4}, expires {m.ExpiryMonth:D2}/{m.ExpiryYear}",
            BuyerOrganisationName = m.BuyerPaymentProfile?.BuyerOrganisation?.Name ?? ""
        }));
    }
}
