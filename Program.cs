using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureBudgetingApp;
using SecureBudgetingApp.Models;

var builder = WebApplication.CreateBuilder(args);

// DB Context
builder.Services.AddDbContext<SecureBudgetingDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";

        // OPTIONAL (recommended)
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// Authorization
builder.Services.AddAuthorization();

// Needed for HttpContext access
builder.Services.AddHttpContextAccessor();

// Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Needed for API calls from Razor
builder.Services.AddHttpClient("", client =>
{
    client.BaseAddress = new Uri("https://localhost:7143"); // adjust if needed
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    UseCookies = true,
    CookieContainer = new System.Net.CookieContainer()
});

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Razor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


// =========================
// LOGIN ENDPOINT
// =========================

app.MapPost("/login-handler", async (HttpContext context, SecureBudgetingDBContext db) =>
{
    var form = await context.Request.ReadFormAsync();

    var email = form["email"];
    var password = form["password"];

    var user = db.Users
        .FirstOrDefault(u => u.Email.ToLower() == email.ToString().ToLower());

    if (user == null)
        return Results.Redirect("/login?error=User not found");

    var hasher = new PasswordHasher<User>();
    var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

    if (result == PasswordVerificationResult.Failed)
        return Results.Redirect("/login?error=Invalid password");

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.Email)
    };

    var identity = new ClaimsIdentity(claims, "Cookies");
    var principal = new ClaimsPrincipal(identity);

    await context.SignInAsync("Cookies", principal);

    return Results.Redirect("/dashboard");
});


// =========================
//  LOGOUT ENDPOINT (NEW)
// =========================

app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync("Cookies");
    return Results.Redirect("/login");
});


app.Run();


// LOGIN REQUEST MODEL

class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}