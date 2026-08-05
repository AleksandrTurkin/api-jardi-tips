using JardiTips.Application.Base;
using JardiTips.Application.Features.Tips;
using JardiTips.Application.Features.Tips.Models;
using JardiTips.Domain.Common;
using JardiTips.WebApi.Endpoints.Base;
using JardiTips.WebApi.Extensions;

namespace JardiTips.WebApi.Endpoints;

public class TipEndpoint : IEndpoint
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetTipByIdQuery, Result<TipDetailDto>>, GetTipByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetTipsQuery, Result<PagedResult<TipDetailDto>>>, GetTipsQueryHandler>();
    }

    public void Map(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/tips").WithTags("Tip");

        group.MapGetByIdQuery<GetTipByIdQuery, TipDetailDto, Guid>("{id:guid}", id => new GetTipByIdQuery(id));
        group.MapGetFilterQuery<GetTipsQuery, PagedResult<TipDetailDto>, TipsFilterDto>("", filters => new GetTipsQuery(filters));
    }
}
