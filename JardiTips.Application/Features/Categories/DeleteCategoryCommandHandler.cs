using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Categories
{
    public record DeleteCategoryCommand(Guid Id);

    public class DeleteCategoryCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<DeleteCategoryCommand, Result>
    {
        public async Task<Result> HandleAsync(DeleteCategoryCommand command, CancellationToken ct = default)
        {
            var repository = unitOfWork.Repository<CategoryEntity>();
            var dbCategory = await repository.GetByIdAsync(command.Id, ct);

            if (dbCategory == null)
                return new ErrorDetail("category-not-found", "The category was not found.", ErrorType.NotFound);  

            repository.Remove(dbCategory);
            await unitOfWork.SaveChangesAsync(ct);

            return Result.Success();        
        }
    }
}
