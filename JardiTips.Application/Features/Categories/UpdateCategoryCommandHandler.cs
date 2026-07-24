using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Categories
{
    public record UpdateCategoryCommand(Guid Id, UpdateCategoryDto Category);

    public class UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IAuthContext authContext) : ICommandHandler<UpdateCategoryCommand, Result>
    {
        public async Task<Result> HandleAsync(UpdateCategoryCommand command, CancellationToken ct = default)
        {
            var userId = authContext.GetUserId();
            var repository = unitOfWork.Repository<CategoryEntity>();
            var dbCategory = await repository.FirstOrDefaultAsync(c => c.Id == command.Id && c.OwnerUserId == userId, ct);

            if (dbCategory == null)
                return new ErrorDetail("category-not-found", "The category was not found.", ErrorType.NotFound);

            dbCategory.Name = command.Category.Name;
            dbCategory.Description = command.Category.Description;
            dbCategory.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
