using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var keycloak = builder.Configuration.GetSection("Keycloak");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(options =>
    {
        options.Authority = RequireValue(keycloak, "Authority");
        options.ClientId = RequireValue(keycloak, "ClientId");
        options.ClientSecret = keycloak["ClientSecret"];

        // Authorization Code flow (с PKCE по умолчанию).
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;

        // Ключаем маппинг, чтобы в principal попадали «сырые» OIDC-клеймы (sub, email, roles и т.д.).
        options.MapInboundClaims = false;
        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "roles";
        
        // В dev-режиме Keycloak поднят по http://localhost:8080.
        options.RequireHttpsMetadata =
            bool.TryParse(keycloak["RequireHttpsMetadata"], out var requireHttps) && requireHttps;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

static string RequireValue(IConfigurationSection section, string key) =>
    section[key] ?? throw new InvalidOperationException(
        $"Configuration section '{section.Path}' must define '{key}'.");
