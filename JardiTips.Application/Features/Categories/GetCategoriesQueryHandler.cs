using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Base;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using System.Linq.Expressions;

namespace JardiTips.Application.Features.Categories
{
    public record GetCategoriesQuery(CategoriesFilterDto Filters);

    public class GetCategoriesQueryHandler(IUnitOfWork unitOfWork, IAuthContext authContext) : BasePagedQueryHandler<CategoriesFilterDto, CategoryEntity>(unitOfWork), IQueryHandler<GetCategoriesQuery, Result<PagedResult<CategoryDto>>>
    {
        public async Task<Result<PagedResult<CategoryDto>>> HandleAsync(GetCategoriesQuery request, CancellationToken ct = default)
        {
            var result = await BaseHandle(request.Filters, Projection, ct);

            return result;
        }

        protected override IQueryable<CategoryEntity> ModifyQuery(IQueryable<CategoryEntity> query, CategoriesFilterDto request)
        {
            return query.Where(x => x.OwnerUserId == null);
        }

        private static readonly Expression<Func<CategoryEntity, CategoryDto>> Projection = category =>
            new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Type = category.Type,
                TipsCount = category.Tips.Count(),
                CoverImageUrl = category.CoverImageUrl,
                UpdatedAt = category.UpdatedAt
            };
    }
}
