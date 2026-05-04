namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class Traveller
{
    public Guid Id { get; set; }
    public Guid ClientOrganisationId { get; set; }
    public Organisation? ClientOrganisation { get; set; }
    public string? ExternalTravellerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DefaultCostCentreCode { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string FullName => $"{FirstName} {LastName}";
}
