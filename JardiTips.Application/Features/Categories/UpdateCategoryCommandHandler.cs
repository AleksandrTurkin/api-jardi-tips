using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Entities;

namespace JardiTips.Application.Features.Categories
{
    public record UpdateCategoryCommand(Guid Id, UpdateCategoryDto Category);

    public class UpdateCategoryCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<UpdateCategoryCommand, bool>
    {
        public async Task<bool> HandleAsync(UpdateCategoryCommand command, CancellationToken ct = default)
        {
            var repository = unitOfWork.Repository<CategoryEntity>();
            var dbCategory = await repository.GetByIdAsync(command.Id, ct);

            if (dbCategory == null)
                throw new Exception("Category not found");

            dbCategory.Name = command.Category.Name;
            dbCategory.Description = command.Category.Description;
            dbCategory.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync(ct);

            return true;
        }
    }
}
