using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/organisations")]
public sealed class OrganisationsController(AppDbContext db) : ControllerBase
{
    [HttpGet("tree")]
    public async Task<ActionResult<List<OrganisationDto>>> Tree()
    {
        var all = await db.Organisations.ToListAsync();
        var roots = all.Where(o => o.ParentOrganisationId == null).ToList();
        return Ok(roots.Select(r => MapTree(r, all)).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrganisationDto>> Get(Guid id)
    {
        var org = await db.Organisations.Include(o => o.ParentOrganisation).FirstOrDefaultAsync(o => o.Id == id);
        if (org == null) return NotFound();
        var all = await db.Organisations.ToListAsync();
        return Ok(MapTree(org, all));
    }

    private static OrganisationDto MapTree(Data.Entities.Organisation o, List<Data.Entities.Organisation> all) => new()
    {
        Id = o.Id,
        Name = o.Name,
        Code = o.Code,
        Type = o.Type,
        ParentOrganisationId = o.ParentOrganisationId,
        ParentOrganisationName = all.FirstOrDefault(x => x.Id == o.ParentOrganisationId)?.Name,
        Children = all.Where(c => c.ParentOrganisationId == o.Id).Select(c => MapTree(c, all)).ToList()
    };
}
