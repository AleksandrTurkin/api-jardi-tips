using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Authentication;

public sealed record RefreshTokenCommand(RefreshTokenDto Token);

public sealed class RefreshTokenCommandHandler(
    IUnitOfWork unitOfWork,
    ITokenService tokenService) : ICommandHandler<RefreshTokenCommand, Result<AuthTokenDto>>
{
    public Task<Result<AuthTokenDto>> HandleAsync(RefreshTokenCommand command, CancellationToken ct = default)
    {
        return unitOfWork.ExecuteTransactionAsync(async () =>
        {
            var tokenHash = tokenService.HashRefreshToken(command.Token.RefreshToken);
            var tokenRepository = unitOfWork.Repository<RefreshTokenEntity>();
            var storedToken = await tokenRepository.FirstOrDefaultAsync(x => x.Token == tokenHash, ct);
            var now = DateTime.UtcNow;

            if (storedToken is null || storedToken.RevokedAt is not null || storedToken.ExpiresAt <= now)
                return Result<AuthTokenDto>.Failure(InvalidRefreshToken());

            var user = await unitOfWork.Repository<UserEntity>().GetByIdAsync(storedToken.UserId, ct);
            if (user is null)
                return Result<AuthTokenDto>.Failure(InvalidRefreshToken());

            storedToken.RevokedAt = now;

            var accessToken = tokenService.CreateAccessToken(user);
            var refreshToken = tokenService.CreateRefreshToken();
            await tokenRepository.AddAsync(new RefreshTokenEntity
            {
                UserId = user.Id,
                Token = tokenService.HashRefreshToken(refreshToken.Value),
                CreatedAt = now,
                ExpiresAt = refreshToken.ExpiresAt
            }, ct);

            await unitOfWork.SaveChangesAsync(ct);

            return Result<AuthTokenDto>.Success(new AuthTokenDto(
                accessToken.Value,
                refreshToken.Value,
                accessToken.ExpiresAt,
                refreshToken.ExpiresAt));
        }, ct);
    }

    private static ErrorDetail InvalidRefreshToken() =>
        new("invalid-refresh-token", "The refresh token is invalid or expired.", ErrorType.Unauthorized);
}
