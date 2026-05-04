using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/audit")]
public sealed class AuditController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuditEventDto>>> GetAll([FromQuery] string? entityType, [FromQuery] Guid? entityId)
    {
        var q = db.AuditEvents.AsQueryable();
        if (!string.IsNullOrEmpty(entityType)) q = q.Where(e => e.EntityType == entityType);
        if (entityId.HasValue) q = q.Where(e => e.EntityId == entityId);
        var list = await q.OrderByDescending(e => e.CreatedAtUtc).Take(200).ToListAsync();
        return Ok(list.Select(e => new AuditEventDto
        {
            Id = e.Id,
            CreatedAtUtc = e.CreatedAtUtc,
            ActorEmail = e.ActorEmail,
            EntityType = e.EntityType,
            EntityId = e.EntityId,
            Action = e.Action,
            Summary = e.Summary
        }));
    }
}
