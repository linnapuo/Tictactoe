using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;
using Quartz;
using Tictactoe.Authentication.Components;
using Tictactoe.Authentication.Components.Account;
using Tictactoe.Authentication.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(descriptions => descriptions.First());

    options.AddSecurityDefinition("OpenID Connect", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OpenIdConnect,
        OpenIdConnectUrl = new Uri("/.well-known/openid-configuration", UriKind.Relative)
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

// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseSqlServer(connectionString);

    options.UseOpenIddict();
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddQuartz(options =>
{
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserNameClaimType = Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = Claims.Role;
    options.ClaimsIdentity.EmailClaimType = Claims.Email;
});

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddOpenIddict()
.AddCore(options =>
{
    options.UseEntityFrameworkCore()
           .UseDbContext<ApplicationDbContext>();

    options.UseQuartz(builder =>
    {
        builder.SetMinimumAuthorizationLifespan(TimeSpan.FromMinutes(10));
        builder.SetMinimumTokenLifespan(TimeSpan.FromMinutes(10));
    });
})
.AddClient(options =>
{
    options.AllowAuthorizationCodeFlow()
           .AllowRefreshTokenFlow();

    options.SetRedirectionEndpointUris("https://localhost:7180/swagger/oauth2-redirect.html");

    // Register the signing and encryption credentials used to protect
    // sensitive data like the state tokens produced by OpenIddict.
    options.AddDevelopmentEncryptionCertificate()
           .AddDevelopmentSigningCertificate();

    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
    options.UseAspNetCore()
           .EnableStatusCodePagesIntegration()
           .EnableRedirectionEndpointPassthrough();

    // Register the System.Net.Http integration and use the identity of the current
    // assembly as a more specific user agent, which can be useful when dealing with
    // providers that use the user agent as a way to throttle requests (e.g Reddit).
    options.UseSystemNetHttp();

    // Register the Web providers integrations.
    //
    // Note: to mitigate mix-up attacks, it's recommended to use a unique redirection endpoint
    // URI per provider, unless all the registered providers support returning a special "iss"
    // parameter containing their URL as part of authorization responses. For more information,
    // see https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
    //options.UseWebProviders()
    //       .AddGitHub(config =>
    //       {
    //           config
    //           .SetClientId("Ov23lihh8KZ7oZxUaP5J")
    //           .SetClientSecret("525d2293a518f90ae41ba25df8317e3b67ceb428")
    //           .SetRedirectUri("https://localhost:7180/swagger/oauth2-redirect.html");
    //       });
})
.AddServer(options =>
{
    options.SetAuthorizationEndpointUris("connect/authorize")
           .SetEndSessionEndpointUris("connect/logout")
           .SetTokenEndpointUris("connect/token")
           .SetUserInfoEndpointUris("connect/userinfo");

    options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

    options.AllowAuthorizationCodeFlow()
           .RequireProofKeyForCodeExchange()
           .AllowClientCredentialsFlow();

    // Register the encryption credentials. This sample uses a symmetric
    // encryption key that is shared between the server and the Api2 sample
    // (that performs local token validation instead of using introspection).
    //
    // Note: in a real world application, this encryption key should be
    // stored in a safe place (e.g in Azure KeyVault, stored as a secret).
    options.AddEncryptionKey(new SymmetricSecurityKey(
        Convert.FromBase64String("dGVzdHRlc3R0ZXN0dGVzdHRlc3R0ZXN0dGVzdHRlc3Q=")));

    options.AddDevelopmentEncryptionCertificate()
           .AddDevelopmentSigningCertificate();

    options.UseAspNetCore()
           .EnableAuthorizationEndpointPassthrough()
           .EnableEndSessionEndpointPassthrough()
           .EnableTokenEndpointPassthrough()
           .EnableUserInfoEndpointPassthrough();

    options.SetAccessTokenLifetime(TimeSpan.FromMinutes(10))
           .SetRefreshTokenLifetime(TimeSpan.FromHours(1));
})
.AddValidation(options =>
{
    // Import the configuration from the local OpenIddict server instance.
    options.UseLocalServer();
    options.UseAspNetCore();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

builder.Services.AddControllers();

builder.Services
    .AddAuthorizationBuilder()
    .AddPolicy("ClientCredentialsPolicy", policy =>
    {
        policy.RequireAuthenticatedUser()
              .RequireClaim(Claims.Scope, "openid")
              .RequireClaim(Claims.ClientId, "swagger")
              .AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    })
    .AddPolicy("AuthorizationCodePolicy", policy =>
    {
        policy.RequireAuthenticatedUser()
              .RequireClaim(Claims.Scope, "openid")
              .RequireClaim(Claims.ClientId, "react", "swagger-code")
              .AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    });

builder.Services.AddHttpLogging();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHttpLogging();
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthUsePkce();
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapControllers();

app.MapGet("/test-client", [Authorize("ClientCredentialsPolicy")] () => Results.Ok("Authorized ok"))
   .WithName("Test Client Credentials Flow");

app.MapGet("/test-authcode", [Authorize("AuthorizationCodePolicy")] (HttpContext httpContext) => Results.Ok("Authorized ok: " + httpContext.User.Identity!.Name))
   .WithName("Test Authorization Code Flow");

await SeedAsync();

app.Run();

async Task SeedAsync()
{
    await using var scope = app.Services.CreateAsyncScope();

    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
    if (await manager.FindByClientIdAsync("swagger") is null)
    {
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "swagger",
            ClientSecret = "swagger",
            RedirectUris =
            {
                new Uri("https://localhost:7180/swagger/oauth2-redirect.html")
            },
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.ClientCredentials
            },
            ClientType = ClientTypes.Confidential
        });
    }
    if (await manager.FindByClientIdAsync("react") is null)
    {
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "react",
            RedirectUris =
            {
                new Uri("http://localhost:5173")
            },
            PostLogoutRedirectUris =
            {
                new Uri("http://localhost:5173")
            },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.EndSession,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.ResponseTypes.Code
            },
            ClientType = ClientTypes.Public,
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            },
            ConsentType = ConsentTypes.Implicit
        });
    }
    if (await manager.FindByClientIdAsync("swagger-code") is null)
    {
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "swagger-code",
            RedirectUris =
            {
                new Uri("https://localhost:7180/swagger/oauth2-redirect.html")
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:7180/swagger/oauth2-redirect.html")
            },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.EndSession,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.ResponseTypes.Code
            },
            ClientType = ClientTypes.Public,
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            },
            ConsentType = ConsentTypes.Implicit
        });
    }
}
