using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Tictactoe.Server;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

//builder.Services.AddOpenApi("v1");

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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:7219")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddSignalR(options =>
{
    options.AddFilter<ExceptionFilter>();
});
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
app.MapControllers();

app.Run();
