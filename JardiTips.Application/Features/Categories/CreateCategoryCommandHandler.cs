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
        //todo move to the user limits service
        private const int MaxUserCategories = 10;

        public async Task<Result<Guid>> HandleAsync(CreateCategoryCommand command, CancellationToken ct = default)
        {
            var repository = unitOfWork.Repository<CategoryEntity>();

            var userId = authContext.GetUserId();
            var category = Map(command);
            category.OwnerUserId = userId;

            //todo move to the user limits service
            var categoryCount = (await repository.GetAllAsync(x => x.OwnerUserId == userId && x.Type == CategoryType.User, ct)).Count();
            if (categoryCount >= MaxUserCategories)
                return new ErrorDetail("category-limit-reached", "You cannot create more than 10 collections.", ErrorType.ValidationError);

            await repository.AddAsync(category, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return category.Id;
        }

        private static CategoryEntity Map(CreateCategoryCommand command)
        {
            return new CategoryEntity
            {
                Name = command.Category.Name,
                Description = command.Category.Description,
                Type = CategoryType.User,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}