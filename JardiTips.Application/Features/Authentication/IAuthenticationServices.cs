using JardiTips.Domain.Entities;

namespace JardiTips.Application.Features.Authentication;

public interface IPasswordService
{
    bool VerifyPassword(UserEntity user, string password);
}

public interface ITokenService
{
    GeneratedToken CreateAccessToken(UserEntity user);

    GeneratedToken CreateRefreshToken();

    string HashRefreshToken(string refreshToken);
}

public sealed record GeneratedToken(string Value, DateTime ExpiresAt);
