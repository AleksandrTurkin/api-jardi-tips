using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;
using System.Linq.Expressions;

namespace JardiTips.Application.Features.Categories
{
    public record GetCategoryByIdQuery(Guid Id);

    public class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IAuthContext authContext) : IQueryHandler<GetCategoryByIdQuery, Result<CategoryDto>>
    {
        private readonly Guid? _userId = authContext.IsAuthenticated() ? authContext.GetUserId() : null;

        public async Task<Result<CategoryDto>> HandleAsync(GetCategoryByIdQuery request, CancellationToken ct = default)
        {
            var repository = unitOfWork.Repository<CategoryEntity>();

            var category = await repository.FirstOrDefaultAsync(
                x => x.Id == request.Id && x.OwnerUserId == null,
                Projection,
                ct);

            if (category == null)
                return new ErrorDetail("category-not-found", $"Category with Id {request.Id} not found.", ErrorType.NotFound);

            return category;
        }

        private Expression<Func<CategoryEntity, CategoryDto>> Projection => category =>
            new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Type = category.Type,
                TipsCount = category.Tips.Count(),
                LikesCount = category.Likes.Count(),
                IsLiked = _userId.HasValue && category.Likes.Any(x => x.UserId == _userId.Value),
                CoverImageUrl = category.CoverImageUrl,
                UpdatedAt = category.UpdatedAt
            };
    }
}
