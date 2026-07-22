using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JardiTips.Application.Features.Authentication;
using JardiTips.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JardiTips.Infrastructure.Authentication;

public sealed class JwtTokenService : ITokenService, IDisposable
{
    private readonly JwtOptions _options;
    private readonly RSA _privateKey;
    private readonly SigningCredentials _signingCredentials;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _privateKey = RSA.Create();
        _privateKey.ImportFromPem(NormalizePem(_options.PrivateKeyPem));
        _signingCredentials = new SigningCredentials(
            new RsaSecurityKey(_privateKey),
            SecurityAlgorithms.RsaSha256);
    }

    public GeneratedToken CreateAccessToken(UserEntity user)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_options.AccessTokenLifetimeMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.DisplayName),
            new Claim("client_id", _options.ClientId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                EpochTime.GetIntDate(now).ToString(),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: _signingCredentials);

        return new GeneratedToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public GeneratedToken CreateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var token = Base64UrlEncoder.Encode(bytes);
        return new GeneratedToken(token, DateTime.UtcNow.AddDays(_options.RefreshTokenLifetimeDays));
    }

    public string HashRefreshToken(string refreshToken)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }

    public void Dispose()
    {
        _privateKey.Dispose();
    }

    public static string NormalizePem(string pem) => pem.Replace("\\n", "\n", StringComparison.Ordinal);
}
