using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Tips.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Tips;

public record CreateTipCommand(CreateTipDto Tip);

public class CreateTipCommandHandler(IUnitOfWork unitOfWork, IAuthContext authContext)
    : ICommandHandler<CreateTipCommand, Result<Guid>>
{
    //todo move to the user limits service
    private const int MaxTipsPerUserCategory = 200;

    public Task<Result<Guid>> HandleAsync(CreateTipCommand command, CancellationToken ct = default)
    {
        return unitOfWork.ExecuteTransactionAsync(async () =>
        {
            var userId = authContext.GetUserId();
            var categoryRepository = unitOfWork.Repository<CategoryEntity>();

            var destination = await categoryRepository.FirstOrDefaultAsync(x => x.Id == command.Tip.CategoryId && x.OwnerUserId == userId, ct);
            if (destination is null)
                return new ErrorDetail("tip-category-not-found", "The destination category was not found.", ErrorType.NotFound);

            var tipRepository = unitOfWork.Repository<TipEntity>();

            //todo move to the user limits service
            var tipsCount = (await tipRepository.GetAllAsync(x => x.CategoryId == destination.Id, ct)).Count();
            if (tipsCount >= MaxTipsPerUserCategory)
                return new ErrorDetail("tip-category-limit-reached", $"The category cannot contain more than {MaxTipsPerUserCategory} tips.", ErrorType.ValidationError);

            var tip = Map(command.Tip);

            await tipRepository.AddAsync(tip, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return Result<Guid>.Success(tip.Id);
        }, ct);
    }

    private static TipEntity Map(CreateTipDto dto)
    {
        var now = DateTime.UtcNow;
        return new TipEntity
        {
            Title = dto.Title,
            Content = dto.Content,
            CategoryId = dto.CategoryId,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

}
