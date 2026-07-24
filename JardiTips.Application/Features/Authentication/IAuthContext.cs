namespace JardiTips.Application.Features.Authentication;

public interface IAuthContext
{
    Guid GetUserId();

    bool IsAuthenticated();
}