using Cinturon360.Mock.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/login";
        o.LogoutPath = "/logout";
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

var apiBase = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:8090";
var apiPublicUrl = builder.Configuration["ApiPublicUrl"] ?? apiBase;
builder.Services.AddHttpClient<ApiClient>(c => c.BaseAddress = new Uri(apiBase));
builder.Services.AddSingleton(new ApiPublicBaseUrl(apiPublicUrl));
builder.Services.AddScoped<SessionState>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<Cinturon360.Mock.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.Run();
