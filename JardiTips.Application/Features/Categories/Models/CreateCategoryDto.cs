using System.ComponentModel.DataAnnotations;

namespace JardiTips.Application.Features.Categories.Models;

public record CreateCategoryDto(
    [property: Required, StringLength(250)] string Name,
    [property: Required, StringLength(2000)] string Description);
