using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Azure;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var azureAdSection = builder.Configuration.GetSection("AzureAd");

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(azureAdSection);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "oauth2",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{azureAdSection["Instance"]}{azureAdSection["TenantId"]}/oauth2/v2.0/authorize"),
                    TokenUrl = new Uri($"{azureAdSection["Instance"]}{azureAdSection["TenantId"]}/oauth2/v2.0/token"),
                    Scopes = new Dictionary<string, string> {
                        { $"{azureAdSection["Audience"]}/{azureAdSection["Scopes"]}", "Access as a user" }
                    }
                }
            }
        });

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "oauth2", //The name of the previously defined security scheme.
                        Type = ReferenceType.SecurityScheme
                    },
                    Scheme = "oauth2",
                    Name = "oauth2",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId(azureAdSection["ClientId"]);
        c.OAuthClientSecret(string.Empty);
        c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        c.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
