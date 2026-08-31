using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Categories
{
    public record GetCategoryByIdQuery(Guid Id);

    public class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IAuthContext authContext) : IQueryHandler<GetCategoryByIdQuery, Result<CategoryDto>>
    {
        public async Task<Result<CategoryDto>> HandleAsync(GetCategoryByIdQuery request, CancellationToken ct = default)
        {
            var repository = unitOfWork.Repository<CategoryEntity>();

            var query = authContext.IsAuthenticated() 
                ? await repository.GetAllAsync(x => x.OwnerUserId == null || x.OwnerUserId == authContext.GetUserId(), ct)
                : await repository.GetAllAsync(x => x.OwnerUserId == null, ct);

            var category = query.FirstOrDefault(x => x.Id == request.Id);

            if (category == null)
                return new ErrorDetail("category-not-found", $"Category with Id {request.Id} not found.", ErrorType.NotFound);
            
            return Map(category);
        }
        
        private static CategoryDto Map(CategoryEntity category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Type = category.Type,
                TipsCount = category.TipsCount,
                CoverImageUrl = category.CoverImageUrl,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}
