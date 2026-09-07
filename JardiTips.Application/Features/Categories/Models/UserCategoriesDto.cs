using JardiTips.Application.Base;

namespace JardiTips.Application.Features.Categories.Models;

public record UserCategoriesDto(
    CategoryDto? Favorite,
    PagedResult<CategoryDto> Categories);
