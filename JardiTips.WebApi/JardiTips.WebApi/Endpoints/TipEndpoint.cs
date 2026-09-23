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
        services.AddScoped<ICommandHandler<CreateTipCommand, Result<Guid>>, CreateTipCommandHandler>();
        services.AddScoped<IQueryHandler<GetTipByIdQuery, Result<TipDetailDto>>, GetTipByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetTipsQuery, Result<PagedResult<TipDetailDto>>>, GetTipsQueryHandler>();
    }

    public void Map(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/tips").WithTags("Tip");

        group.MapPostCommand<CreateTipCommand, CreateTipDto>("", dto => new CreateTipCommand(dto));
        group.MapGetByIdAnonymousQuery<GetTipByIdQuery, TipDetailDto, Guid>("{id:guid}", id => new GetTipByIdQuery(id));
        group.MapGetFilterAnonymousQuery<GetTipsQuery, PagedResult<TipDetailDto>, TipsFilterDto>("", filters => new GetTipsQuery(filters));
    }
}
