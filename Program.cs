using LibararyWebApplication.Components;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PRNLibrary.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddDbContext<PrnContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


// JWT AUTHENTICATION
var jwtSettings_1 = builder.Configuration.GetSection("Jwt");
var jwtSettings = jwtSettings_1.Get<JwtSettings>();

if (jwtSettings == null)
{
    throw new InvalidOperationException("JWT settings are not configured properly.");
}

builder.Services.Configure<JwtSettings>(jwtSettings_1);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});
builder.Services.AddSingleton<TokenService>();

// EMAIL SERVICE
var emailSettingsSection = builder.Configuration.GetSection("EmailServiceSettings");
if (emailSettingsSection == null)
{
    throw new InvalidOperationException("Email service settings are not configured properly.");
}

builder.Services.Configure<EmailServiceSettings>(emailSettingsSection);
builder.Services.AddSingleton<EmailService>();

// RAZOR PAGES
builder.Services.AddRazorPages();
// BLAZOR COMPONENTS
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
