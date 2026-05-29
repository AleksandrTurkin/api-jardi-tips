using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Domain.Entities;

namespace JardiTips.Application.Features.Categories
{
    public record DeleteCategoryCommand(Guid Id);

    public class DeleteCategoryCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<DeleteCategoryCommand, bool>
    {
        public async Task<bool> HandleAsync(DeleteCategoryCommand command, CancellationToken ct = default)
        {
            var repository = unitOfWork.Repository<CategoryEntity>();
            var dbCategory = await repository.GetByIdAsync(command.Id, ct);

            if (dbCategory == null)
                throw new Exception("Category not found");

            repository.Remove(dbCategory);
            await unitOfWork.SaveChangesAsync(ct);

            return true;        
        }
    }
}
