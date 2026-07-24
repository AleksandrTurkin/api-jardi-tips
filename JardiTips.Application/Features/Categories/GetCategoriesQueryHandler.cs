using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Base;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;

namespace JardiTips.Application.Features.Categories
{
    public record GetCategoriesQuery(CategoriesFilterDto Filters);

    public class GetCategoriesQueryHandler(IUnitOfWork unitOfWork, IAuthContext authContext) : BasePagedQueryHandler<CategoriesFilterDto, CategoryEntity>(unitOfWork), IQueryHandler<GetCategoriesQuery, Result<PagedResult<CategoryDto>>>
    {
        public async Task<Result<PagedResult<CategoryDto>>> HandleAsync(GetCategoriesQuery query, CancellationToken ct = default)
        {
            var result = await BaseHandle(query.Filters, Map, ct);

            return result;
        }

        protected override IQueryable<CategoryEntity> ModifyQuery(IQueryable<CategoryEntity> query, CategoriesFilterDto request)
        {
            var userId = authContext.GetUserId();
            return query.Where(x => x.OwnerUserId == userId || x.OwnerUserId == null);
        }

        private static CategoryDto Map(CategoryEntity category) 
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Type = category.Type
            };
        }   
    }
}
