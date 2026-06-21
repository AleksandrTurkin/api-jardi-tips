using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;

namespace JardiTips.Application.Features.Categories
{
    public record GetCategoriesQuery(CategoriesFilterDto Filters);

    public class GetCategoriesQueryHandler(IUnitOfWork unitOfWork) : IQueryHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
    {
        public async Task<Result<List<CategoryDto>>> HandleAsync(GetCategoriesQuery query, CancellationToken ct = default)
        {
            var repository = unitOfWork.Repository<CategoryEntity>();
            var categories = await repository.GetAllAsync(ct);

            return categories.Select(Map).ToList();
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
