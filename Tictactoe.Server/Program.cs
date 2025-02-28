using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Tictactoe.Server;
using Microsoft.Azure.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(descriptions => descriptions.First());

    options.AddSecurityDefinition("OpenID Connect", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OpenIdConnect,
        OpenIdConnectUrl = new Uri("https://localhost:7180/.well-known/openid-configuration", UriKind.Absolute)
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "OpenID Connect"
                }
            },
            new List<string>()
        }
    });
});

var allowedOrigins = builder.Configuration
    .GetValue<string>("AllowedOrigins")
    ?.Split(";") ?? throw new InvalidOperationException("Missing configuration AllowedOrigins");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var signalr = builder.Services.AddSignalR(options =>
{
    options.AddFilter<ExceptionFilter>();
});

if (builder.Environment.IsProduction())
{
    signalr.AddAzureSignalR();
}

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddOpenIddict().AddValidation(options =>
{
    // Note: the validation handler uses OpenID Connect discovery
    // to retrieve the issuer signing keys used to validate tokens.
    options.SetIssuer("https://localhost:7180/");
    //options.AddAudiences("tictactoe_server_1");

    options.UseIntrospection()
        .SetClientId("tictactoe_server_1")
        .SetClientSecret("tictactoe_server_1");

    // Register the System.Net.Http integration.
    options.UseSystemNetHttp();

    // Register the ASP.NET Core host.
    options.UseAspNetCore();
});

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

var apiKey = builder.Configuration
    .GetValue<string>("ApiKey")
    ?? throw new InvalidOperationException("Missing configuration ApiKey");

builder.Services.AddTransient(sp => new ApiKeyMiddleware(apiKey));

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

app.MapHub<GameHub>("/gamehub");
app.MapHub<ChatHub>("/chathub");

if (app.Environment.IsDevelopment())
{
    app.MapControllers();
}

app.Run();
