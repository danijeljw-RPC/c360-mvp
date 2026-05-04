using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Api.Data.Entities;

namespace Cinturon360.Mock.Api.Services;

public sealed class AuditService(AppDbContext db)
{
    public async Task LogAsync(string actorEmail, string entityType, Guid? entityId, string action, string summary, Guid? organisationId = null, string? jsonDetails = null)
    {
        db.AuditEvents.Add(new AuditEvent
        {
            Id = Guid.NewGuid(),
            CreatedAtUtc = DateTimeOffset.UtcNow,
            ActorEmail = actorEmail,
            OrganisationId = organisationId,
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            Summary = summary,
            JsonDetails = jsonDetails
        });
        await db.SaveChangesAsync();
    }
}
