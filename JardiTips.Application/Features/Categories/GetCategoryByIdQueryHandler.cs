using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Categories
{
    public record GetCategoryByIdQuery(Guid Id);

    public class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork) : IQueryHandler<GetCategoryByIdQuery, Result<CategoryDto>>
    {
        public async Task<Result<CategoryDto>> HandleAsync(GetCategoryByIdQuery query, CancellationToken ct = default)
        {
            var repository = unitOfWork.Repository<CategoryEntity>();
            var category = await repository.GetByIdAsync(query.Id, ct);

            if (category == null)
                return new ErrorDetail("Category.NotFound", $"Category with Id {query.Id} not found.", ErrorType.NotFound);
            
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
