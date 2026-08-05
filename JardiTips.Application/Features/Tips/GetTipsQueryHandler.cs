using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Base;
using JardiTips.Application.Features.Tips.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;

namespace JardiTips.Application.Features.Tips;

public record GetTipsQuery(TipsFilterDto Filters);

public class GetTipsQueryHandler(IUnitOfWork unitOfWork, IAuthContext authContext)
    : BasePagedQueryHandler<TipsFilterDto, TipEntity>(unitOfWork),
        IQueryHandler<GetTipsQuery, Result<PagedResult<TipDetailDto>>>
{
    public async Task<Result<PagedResult<TipDetailDto>>> HandleAsync(GetTipsQuery query, CancellationToken ct = default)
    {
        var result = await BaseHandle(query.Filters, Map, ct);

        return result;
    }

    protected override IQueryable<TipEntity> ModifyQuery(IQueryable<TipEntity> query, TipsFilterDto request)
    {
        var userId = authContext.GetUserId();
        return query.Where(x =>
            x.CategoryId == request.CategoryId &&
            (x.Category.OwnerUserId == null || x.Category.OwnerUserId == userId));
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
