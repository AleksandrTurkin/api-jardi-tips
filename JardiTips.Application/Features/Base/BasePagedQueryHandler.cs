using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Domain.Entities;

namespace JardiTips.Application.Features.Base;

public abstract class BasePagedQueryHandler<TQuery, TEntity>(IUnitOfWork unitOfWork)
    where TEntity : BaseEntity
    where TQuery : PagedRequestDto
{
    private readonly int DefaultLimit = 15;

    protected async Task<PagedResult<TDto>> BaseHandle<TDto>(TQuery request, Func<TEntity, TDto> map, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<TEntity>();

        var query = repository.GetAll();
        query = ModifyQuery(query, request);


        var pageContext = ParsePageContext(request.PageContext);

        var limit = request.Limit ?? DefaultLimit;
        var items = (await repository.GetPagerResultAsync(query, pageContext.DateTime, pageContext.LastId, limit + 1, cancellationToken)).ToList();

        var hasMore = items.Count > limit;
        if (hasMore)
            items.RemoveAt(items.Count - 1);

        var cursor = hasMore
            ? PagedCursor.Encode(items[^1].CreatedAt, items[^1].Id)
            : null;
        
        return new PagedResult<TDto>(cursor, items.Select(map).ToList());
    }

    private (DateTime? DateTime, Guid? LastId) ParsePageContext(string? pageContext)
    {
        if (string.IsNullOrWhiteSpace(pageContext))
            return (null, null);

        var context = PagedCursor.Decode(pageContext);
        
        if (context == null)
            return (null, null);

        return (context.DateTime, context.LastId);
    }

    protected virtual IQueryable<TEntity> ModifyQuery(IQueryable<TEntity> query, TQuery request) => query;
}

