using System.Security.Claims;
using System.Text;
using MinimalApiAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MinimalApiAuth.Models;
using MinimalApiAuth.Repositories;
using MinimalApiAuth.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(Settings.Secret);
builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false // teste
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("manager"));
    options.AddPolicy("Employee", policy => policy.RequireRole("employee"));
});

var app = builder.Build();
app.UseAuthentication(); // has to be in this order due th pipeline of the application
app.UseAuthorization();

app.MapPost("/login", (User model) =>
{
    var user = UserRepository.Get(model.UserName, model.Password);
    if (user == null)
    {
        return Results.NotFound(new { message = "Usuario ou senha invalidos" });
    }

    var token = TokenService.GenerateToken(user);
    user.Password = "";

    return  Results.Ok( new 
    {
        user = user,
        token = token

    });
});

app.MapGet("/anonymous", () => Results.Ok("anonymous")).AllowAnonymous();

app.MapGet("/authenticated", (ClaimsPrincipal user) =>
{
    Results.Ok(new { message = $"Authenticated as {user.Identity.Name}" });
    
}).RequireAuthorization();

app.MapGet("/employee", (ClaimsPrincipal user) =>
{
    Results.Ok(new { message = $"Authenticated as {user.Identity.Name}" });
    
}).RequireAuthorization("Employee");

app.MapGet("/manager", (ClaimsPrincipal user) =>
{
    Results.Ok(new { message = $"Authenticated as {user.Identity.Name}" });
    
}).RequireAuthorization("Admin");

app.Run();
