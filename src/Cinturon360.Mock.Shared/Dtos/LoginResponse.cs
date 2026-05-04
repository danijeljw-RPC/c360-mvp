namespace Cinturon360.Mock.Shared.Dtos;
public sealed class LoginResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? UserEmail { get; set; }
    public string? UserRole { get; set; }
    public string? OrganisationName { get; set; }
}
