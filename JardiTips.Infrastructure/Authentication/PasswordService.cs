using JardiTips.Application.Features.Authentication;
using JardiTips.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace JardiTips.Infrastructure.Authentication;

public sealed class PasswordService : IPasswordService
{
    private readonly PasswordHasher<UserEntity> _passwordHasher = new();

    public bool VerifyPassword(UserEntity user, string password)
    {
        if (user.PasswordHash is null)
            return false;

        return _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password)
            is not PasswordVerificationResult.Failed;
    }
}
