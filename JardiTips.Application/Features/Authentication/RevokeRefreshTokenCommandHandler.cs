using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;

namespace JardiTips.Application.Features.Authentication;

public sealed record RevokeRefreshTokenCommand(RefreshTokenDto Token);

public sealed class RevokeRefreshTokenCommandHandler(
    IUnitOfWork unitOfWork,
    ITokenService tokenService) : ICommandHandler<RevokeRefreshTokenCommand, Result>
{
    public async Task<Result> HandleAsync(RevokeRefreshTokenCommand command, CancellationToken ct = default)
    {
        var tokenHash = tokenService.HashRefreshToken(command.Token.RefreshToken);
        var storedToken = await unitOfWork.Repository<RefreshTokenEntity>()
            .FirstOrDefaultAsync(x => x.Token == tokenHash, ct);

        if (storedToken is null || storedToken.RevokedAt is not null)
            return Result.Success();

        storedToken.RevokedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
