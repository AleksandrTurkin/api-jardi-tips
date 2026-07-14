namespace JardiTips.Application.Base
{
    public record PagedResult<T>(string? PageContext, List<T> Data);
}
