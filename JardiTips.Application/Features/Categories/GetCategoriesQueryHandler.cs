using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Base;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;

namespace JardiTips.Application.Features.Categories
{
    public record GetCategoriesQuery(CategoriesFilterDto Filters);

    public class GetCategoriesQueryHandler(IUnitOfWork unitOfWork) : BasePagedQueryHandler<CategoriesFilterDto, CategoryEntity>(unitOfWork), IQueryHandler<GetCategoriesQuery, Result<PagedResult<CategoryDto>>>
    {
        public async Task<Result<PagedResult<CategoryDto>>> HandleAsync(GetCategoriesQuery query, CancellationToken ct = default)
        {
            var result = await BaseHandle(query.Filters, Map, ct);

            return result;
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
