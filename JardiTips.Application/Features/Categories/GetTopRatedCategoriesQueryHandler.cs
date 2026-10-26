using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;

namespace JardiTips.Application.Features.Categories
{
    public record GetTopRatedCategoriesQuery();

    public class GetTopRatedCategoriesQueryHandler(IUnitOfWork unitOfWork, IAuthContext authContext)
        : IQueryHandler<GetTopRatedCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
    {
        //todo : move to configuration
        private const int TopCount = 10;
        private readonly Guid? _userId = authContext.IsAuthenticated() ? authContext.GetUserId() : null;

        public async Task<Result<IReadOnlyList<CategoryDto>>> HandleAsync(GetTopRatedCategoriesQuery request, CancellationToken ct = default)
        {
            var repository = unitOfWork.Repository<CategoryEntity>();
            var query = repository.Where(category => category.OwnerUserId == null)
                .OrderByDescending(category => category.Likes.Count())
                .ThenByDescending(category => category.CreatedAt)
                .ThenByDescending(category => category.Id)
                .Take(TopCount);

            var categories = await repository.GetListAsync(query, CategoryProjection.ToDto(_userId), ct);

            return Result<IReadOnlyList<CategoryDto>>.Success(categories);
        }
    }
}
