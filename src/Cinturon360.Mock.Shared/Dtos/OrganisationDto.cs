using Cinturon360.Mock.Shared.Enums;
namespace Cinturon360.Mock.Shared.Dtos;
public sealed class OrganisationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public OrganisationType Type { get; set; }
    public Guid? ParentOrganisationId { get; set; }
    public string? ParentOrganisationName { get; set; }
    public List<OrganisationDto> Children { get; set; } = [];
}
