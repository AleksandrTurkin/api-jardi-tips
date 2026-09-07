using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Base;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Categories;

public record GetUserCategoriesQuery(CategoriesFilterDto Filters);

public class GetUserCategoriesQueryHandler(IUnitOfWork unitOfWork, IAuthContext authContext) : BasePagedQueryHandler<CategoriesFilterDto, CategoryEntity>(unitOfWork), IQueryHandler<GetUserCategoriesQuery, Result<UserCategoriesDto>>
{
    public async Task<Result<UserCategoriesDto>> HandleAsync(
        GetUserCategoriesQuery request,
        CancellationToken ct = default)
    {
        var userId = authContext.GetUserId();
        var repository = unitOfWork.Repository<CategoryEntity>();

        var favorite = await repository.FirstOrDefaultAsync(x => x.OwnerUserId == userId && x.Type == CategoryType.Favorites, ct);

        var categories = await BaseHandle(request.Filters, Map, ct);

        return new UserCategoriesDto(
            favorite is null ? null : Map(favorite), 
            categories);
    }

    protected override IQueryable<CategoryEntity> ModifyQuery(IQueryable<CategoryEntity> query, CategoriesFilterDto request)
    {
        var userId = authContext.GetUserId();
        return query.Where(x => x.OwnerUserId == userId && x.Type == CategoryType.User);
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
