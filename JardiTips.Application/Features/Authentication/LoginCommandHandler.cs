using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Authentication;

public sealed record LoginCommand(LoginDto Login);

public sealed class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordService passwordService,
    ITokenService tokenService) : ICommandHandler<LoginCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> HandleAsync(LoginCommand command, CancellationToken ct = default)
    {
        var normalizedEmail = command.Login.Email.Trim().ToLowerInvariant();
        var userRepository = unitOfWork.Repository<UserEntity>();
        var user = await userRepository.FirstOrDefaultAsync(
            x => x.Email.ToLower() == normalizedEmail,
            ct);

        if (user?.PasswordHash is null || !passwordService.VerifyPassword(user, command.Login.Password))
            return InvalidCredentials();

        var accessToken = tokenService.CreateAccessToken(user);
        var refreshToken = tokenService.CreateRefreshToken();
        var refreshTokenEntity = new RefreshTokenEntity
        {
            UserId = user.Id,
            Token = tokenService.HashRefreshToken(refreshToken.Value),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = refreshToken.ExpiresAt
        };

        await unitOfWork.Repository<RefreshTokenEntity>().AddAsync(refreshTokenEntity, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthTokenDto(
            accessToken.Value,
            refreshToken.Value,
            accessToken.ExpiresAt,
            refreshToken.ExpiresAt);
    }

    private static ErrorDetail InvalidCredentials() =>
        new("invalid-credentials", "The email or password is invalid.", ErrorType.Unauthorized);
}
