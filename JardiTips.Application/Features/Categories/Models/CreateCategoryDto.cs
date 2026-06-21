using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Categories.Models
{
    public record CreateCategoryDto(string Name, string Description, CategoryType Type);
}
