using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AppDbContext db) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        var user = await db.Users
            .Include(u => u.Organisation)
            .FirstOrDefaultAsync(u => u.Email == req.Email);

        if (user == null)
            return Ok(new LoginResponse { Success = false, ErrorMessage = "Invalid email or password" });

        // Verify bcrypt hash
        bool valid = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!valid)
            return Ok(new LoginResponse { Success = false, ErrorMessage = "Invalid email or password" });

        return Ok(new LoginResponse
        {
            Success = true,
            UserEmail = user.Email,
            UserRole = user.Role.ToString(),
            OrganisationName = user.Organisation?.Name
        });
    }
}
