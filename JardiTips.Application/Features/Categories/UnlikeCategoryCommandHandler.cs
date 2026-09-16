using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Categories;

public record UnlikeCategoryCommand(Guid CategoryId);

public class UnlikeCategoryCommandHandler(IUnitOfWork unitOfWork, IAuthContext authContext)
    : ICommandHandler<UnlikeCategoryCommand, Result>
{
    public async Task<Result> HandleAsync(UnlikeCategoryCommand command, CancellationToken ct = default)
    {
        var userId = authContext.GetUserId();
        var categoryRepository = unitOfWork.Repository<CategoryEntity>();

        var categoryExists = await categoryRepository.AnyAsync(
            x => x.Id == command.CategoryId && x.OwnerUserId == null,
            ct);

        if (!categoryExists)
            return new ErrorDetail("category-not-found", "The category was not found.", ErrorType.NotFound);

        var likeRepository = unitOfWork.Repository<CategoryLikeEntity>();
        var like = await likeRepository.FirstOrDefaultAsync(
            x => x.CategoryId == command.CategoryId && x.UserId == userId,
            ct);

        if (like == null)
            return Result.Success();

        likeRepository.Remove(like);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
