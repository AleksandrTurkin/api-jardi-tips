using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Tips.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Tips;

public record GetTipByIdQuery(Guid Id);

public class GetTipByIdQueryHandler(IUnitOfWork unitOfWork, IAuthContext authContext)
    : IQueryHandler<GetTipByIdQuery, Result<TipDetailDto>>
{
    public async Task<Result<TipDetailDto>> HandleAsync(GetTipByIdQuery query, CancellationToken ct = default)
    {
        var userId = authContext.GetUserId();
        var repository = unitOfWork.Repository<TipEntity>();
        var tip = await repository.FirstOrDefaultAsync(
            x => x.Id == query.Id && (x.Category.OwnerUserId == null || x.Category.OwnerUserId == userId),
            ct);

        if (tip == null)
            return new ErrorDetail("tip-not-found", $"Tip with Id {query.Id} not found.", ErrorType.NotFound);

        return Map(tip);
    }

    private static TipDetailDto Map(TipEntity tip)
    {
        return new TipDetailDto
        {
            Id = tip.Id,
            Title = tip.Title,
            Content = tip.Content,
            CategoryId = tip.CategoryId,
            CreatedAt = tip.CreatedAt,
            UpdatedAt = tip.UpdatedAt
        };
    }
}
