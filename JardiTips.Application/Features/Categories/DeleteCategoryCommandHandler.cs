using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Categories;

public record DeleteCategoryCommand(Guid Id);

public class DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, IAuthContext authContext) : ICommandHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteCategoryCommand command, CancellationToken ct = default)
    {
        var userId = authContext.GetUserId();

        var repository = unitOfWork.Repository<CategoryEntity>();
        var dbCategory = await repository.FirstOrDefaultAsync(x => x.Id == command.Id && x.OwnerUserId == userId, ct);

        if (dbCategory == null)
            return new ErrorDetail("category-not-found", "The category was not found.", ErrorType.NotFound);  

        repository.Remove(dbCategory);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();        
    }
}

