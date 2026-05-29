using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Entities;

namespace JardiTips.Application.Features.Categories
{
    public record CreateCategoryCommand(CreateCategoryDto Category);

    public class CreateCategoryCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<CreateCategoryCommand, Guid>
    {
        public async Task<Guid> HandleAsync(CreateCategoryCommand command, CancellationToken ct = default)
        {
            var repository = unitOfWork.Repository<CategoryEntity>();
            var category = Map(command);

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
                Type = command.Category.Type,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}