using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseSqlServer(connectionString, builder =>
    {
        builder.MigrationsAssembly("Tictactoe.Authentication");
    });

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

// TODO: check this later
builder.Services
    .AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddOpenIddict()
.AddCore(options =>
{
    options.UseEntityFrameworkCore()
           .UseDbContext<IdentityDbContext>();

    options.UseQuartz(builder =>
    {
        builder.SetMinimumAuthorizationLifespan(TimeSpan.FromMinutes(10));
        builder.SetMinimumTokenLifespan(TimeSpan.FromMinutes(10));
    });
})
.AddServer(options =>
{
    options.SetAuthorizationEndpointUris("connect/authorize")
           .SetEndSessionEndpointUris("connect/logout")
           //.SetIntrospectionEndpointUris("connect/introspect")
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
})
.AddValidation(options =>
{
    // Import the configuration from the local OpenIddict server instance.
    options.UseLocalServer();
    options.UseAspNetCore();
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserNameClaimType = Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = Claims.Role;
    options.ClaimsIdentity.EmailClaimType = Claims.Email;
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
        policy
            .RequireAuthenticatedUser()
            .RequireClaim(Claims.Scope, "openid")
            .AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    })
    .AddPolicy("AuthorizationCodePolicy", policy =>
    {
        policy
            .RequireAuthenticatedUser()
            .RequireClaim(Claims.Scope, "openid")
            //.RequireUserName("test")
            .AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    });

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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

app.Run();
