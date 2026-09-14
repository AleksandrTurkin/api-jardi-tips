namespace JardiTips.Application.Base;

public interface IPagedItemDto
{
    Guid Id { get; }

    DateTime UpdatedAt { get; }
}
