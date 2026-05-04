namespace Cinturon360.Mock.Web.Services;

public sealed class SessionState
{
    public string? UserEmail { get; set; }
    public string? UserRole { get; set; }
    public string? OrganisationName { get; set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(UserEmail);
}
