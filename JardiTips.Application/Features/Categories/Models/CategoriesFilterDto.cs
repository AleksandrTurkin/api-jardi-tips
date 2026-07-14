using System.ComponentModel.DataAnnotations;
using JardiTips.Application.Base;

namespace JardiTips.Application.Features.Categories.Models
{
    public record CategoriesFilterDto(
        string? PageContext,
        int? Limit) 
    : PagedRequestDto(PageContext, Limit);
}
