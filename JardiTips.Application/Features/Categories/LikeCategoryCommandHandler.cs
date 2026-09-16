using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Categories;

public record LikeCategoryCommand(Guid CategoryId);

public class LikeCategoryCommandHandler(IUnitOfWork unitOfWork, IAuthContext authContext)
    : ICommandHandler<LikeCategoryCommand, Result>
{
    public async Task<Result> HandleAsync(LikeCategoryCommand command, CancellationToken ct = default)
    {
        var userId = authContext.GetUserId();
        var categoryRepository = unitOfWork.Repository<CategoryEntity>();

        var categoryExists = await categoryRepository.AnyAsync(
            x => x.Id == command.CategoryId && x.OwnerUserId == null,
            ct);

        if (!categoryExists)
            return new ErrorDetail("category-not-found", "The category was not found.", ErrorType.NotFound);

        var likeRepository = unitOfWork.Repository<CategoryLikeEntity>();
        var likeExists = await likeRepository.AnyAsync(
            x => x.CategoryId == command.CategoryId && x.UserId == userId,
            ct);

        if (likeExists)
            return Result.Success();

        await likeRepository.AddAsync(new CategoryLikeEntity
        {
            CategoryId = command.CategoryId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        }, ct);

        try
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch
        {
            likeExists = await likeRepository.AnyAsync(x => x.CategoryId == command.CategoryId && x.UserId == userId, ct);

            if (!likeExists)
                throw;
        }

        return Result.Success();
    }
}
