using System.Security.Cryptography;
using JardiTips.Application.Features.Authentication;
using JardiTips.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JardiTips.WebApi.Extensions;

public static class AuthenticationExtensions
{
    private const string ProblemTypeBaseUri = "/errors/";

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "Jwt:Issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "Jwt:Audience is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.ClientId), "Jwt:ClientId is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.PrivateKeyPem), "Jwt:PrivateKeyPem is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.PublicKeyPem), "Jwt:PublicKeyPem is required.")
            .Validate(options => options.AccessTokenLifetimeMinutes > 0, "Jwt:AccessTokenLifetimeMinutes must be greater than 0.")
            .Validate(options => options.RefreshTokenLifetimeDays > 0, "Jwt:RefreshTokenLifetimeDays must be greater than 0.")
            .ValidateOnStart();

        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthContext, AuthContext>();

        var jwtOptions = GetRequiredJwtOptions(configuration);
        var publicKey = RSA.Create();
        publicKey.ImportFromPem(JwtTokenService.NormalizePem(jwtOptions.PublicKeyPem));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(publicKey),
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        return WriteProblemAsync(
                            context.HttpContext,
                            StatusCodes.Status401Unauthorized,
                            "unauthorized",
                            "Unauthorized",
                            "Authentication is required or the provided token is invalid.");
                    },
                    OnForbidden = context =>
                        WriteProblemAsync(
                            context.HttpContext,
                            StatusCodes.Status403Forbidden,
                            "forbidden",
                            "Forbidden",
                            "You do not have permission to access this resource.")
                };
            });

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        return services;
    }

    private static JwtOptions GetRequiredJwtOptions(IConfiguration configuration)
    {
        var options = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

        if (string.IsNullOrWhiteSpace(options.Issuer) ||
            string.IsNullOrWhiteSpace(options.Audience) ||
            string.IsNullOrWhiteSpace(options.ClientId) ||
            string.IsNullOrWhiteSpace(options.PrivateKeyPem) ||
            string.IsNullOrWhiteSpace(options.PublicKeyPem))
        {
            throw new InvalidOperationException("JWT authentication is not configured. Set Jwt:Issuer, Jwt:Audience, Jwt:ClientId, Jwt:PrivateKeyPem, and Jwt:PublicKeyPem.");
        }

        return options;
    }

    private static Task WriteProblemAsync(
        HttpContext httpContext,
        int statusCode,
        string code,
        string title,
        string detail)
    {
        httpContext.Response.StatusCode = statusCode;

        var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

        return problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Type = ProblemTypeBaseUri + code,
                Title = title,
                Detail = detail,
                Status = statusCode,
                Instance = httpContext.Request.Path,
                Extensions = { ["code"] = code }
            }
        }).AsTask();
    }
}
