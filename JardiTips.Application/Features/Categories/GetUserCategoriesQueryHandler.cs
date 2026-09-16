using JardiTips.Application.Base;
using JardiTips.Application.DataAccess;
using JardiTips.Application.Features.Authentication;
using JardiTips.Application.Features.Base;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Common;
using JardiTips.Domain.Entities;
using JardiTips.Domain.Enums;
using System.Linq.Expressions;

namespace JardiTips.Application.Features.Categories;

public record GetUserCategoriesQuery(CategoriesFilterDto Filters);

public class GetUserCategoriesQueryHandler(IUnitOfWork unitOfWork, IAuthContext authContext) : BasePagedQueryHandler<CategoriesFilterDto, CategoryEntity>(unitOfWork), IQueryHandler<GetUserCategoriesQuery, Result<UserCategoriesDto>>
{
    private readonly Guid _userId = authContext.GetUserId();

    public async Task<Result<UserCategoriesDto>> HandleAsync(
        GetUserCategoriesQuery request,
        CancellationToken ct = default)
    {
        var repository = unitOfWork.Repository<CategoryEntity>();

        var favorite = await repository.FirstOrDefaultAsync(
            x => x.OwnerUserId == _userId && x.Type == CategoryType.Favorites,
            Projection,
            ct);

        var categories = await BaseHandle(request.Filters, Projection, ct);

        return new UserCategoriesDto(favorite, categories);
    }

    protected override IQueryable<CategoryEntity> ModifyQuery(IQueryable<CategoryEntity> query, CategoriesFilterDto request)
    {
        return query.Where(x => x.OwnerUserId == _userId && x.Type == CategoryType.User);
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
