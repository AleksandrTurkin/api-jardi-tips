using System.Security.Claims;
using JardiTips.Application.Features.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace JardiTips.Infrastructure.Authentication;

public class AuthContext(IHttpContextAccessor httpContextAccessor) : IAuthContext
{
    public Guid GetUserId()
    {
        var user = GetUser();

        var userIdClaim = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User id claim is missing");

        return Guid.TryParse(userIdClaim, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("User id claim is not a valid identifier");
    }

    public bool IsAuthenticated()
    {
        return httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }

    private ClaimsPrincipal GetUser()
    {
        var user = httpContextAccessor.HttpContext?.User;

        return user ?? throw new UnauthorizedAccessException("User is not authenticated");
    }
}

