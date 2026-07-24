using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Categories
{
    public record CreateCategoryCommand(CreateCategoryDto Category);

    public class CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IAuthContext authContext) : ICommandHandler<CreateCategoryCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> HandleAsync(CreateCategoryCommand command, CancellationToken ct = default)
        {
            var validationResult = Validate(command);
            if (validationResult.IsFailure)
                return validationResult.Error!;

            var repository = unitOfWork.Repository<CategoryEntity>();

            var favoriteCategory = await GetFavoriteCategory(command, repository, ct);
            if (favoriteCategory != null)
                return favoriteCategory.Id;

            var userId = authContext.GetUserId();
            var category = Map(command);
            category.OwnerUserId = userId;

            await repository.AddAsync(category, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return category.Id;
        }

        private static Result Validate(CreateCategoryCommand command)
        {
            if (command.Category.Type == CategoryType.System)
                return new ErrorDetail("category-type-not-allowed", "Categories of type 'System' cannot be created by users.", ErrorType.ValidationError);

            return Result.Success();
        }

        private async Task<CategoryEntity?> GetFavoriteCategory(CreateCategoryCommand command, IRepository<CategoryEntity> repository, CancellationToken ct)
        {
            if (command.Category.Type != CategoryType.Favorites)
                return null;

            var userId = authContext.GetUserId();
            var dbFavoriteCategory = await repository.FirstOrDefaultAsync(r => r.OwnerUserId == userId && r.Type == CategoryType.Favorites, ct);
            return dbFavoriteCategory;
        }

        private static CategoryEntity Map(CreateCategoryCommand command)
        {
            return new CategoryEntity
            {
                Name = command.Category.Name,
                Description = command.Category.Description,
                Type = command.Category.Type,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}