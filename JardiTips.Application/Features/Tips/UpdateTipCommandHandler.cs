using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Tips.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Tips;

public record UpdateTipCommand(Guid Id, UpdateTipDto Tip);

public class UpdateTipCommandHandler(IUnitOfWork unitOfWork, IAuthContext authContext)
    : ICommandHandler<UpdateTipCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateTipCommand command, CancellationToken ct = default)
    {
        var userId = authContext.GetUserId();
        var repository = unitOfWork.Repository<TipEntity>();
        var tip = await repository.FirstOrDefaultAsync(
            x => x.Id == command.Id && x.Category.OwnerUserId == userId, ct);

        if (tip is null)
            return new ErrorDetail("tip-not-found", "The tip was not found.", ErrorType.NotFound);

        tip.Title = command.Tip.Title;
        tip.Content = command.Tip.Content;
        tip.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
