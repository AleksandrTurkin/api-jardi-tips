using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Entities;

namespace JardiTips.Application.Features.Categories
{
    public record GetCategoryByIdQuery(Guid Id);

    public class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork) : IQueryHandler<GetCategoryByIdQuery, CategoryDto>
    {
        public async Task<CategoryDto> HandleAsync(GetCategoryByIdQuery query, CancellationToken ct = default)
        {
            var repository = unitOfWork.Repository<CategoryEntity>();
            var category = await repository.GetByIdAsync(query.Id, ct);

            if (category == null)
                throw new Exception($"Category with Id {query.Id} not found.");
            
            return Map(category);
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
