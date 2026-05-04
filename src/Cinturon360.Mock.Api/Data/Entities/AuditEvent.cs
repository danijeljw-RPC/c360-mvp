namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class AuditEvent
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string ActorEmail { get; set; } = string.Empty;
    public Guid? OrganisationId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? JsonDetails { get; set; }
}
