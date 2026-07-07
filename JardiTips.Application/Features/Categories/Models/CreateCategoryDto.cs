using System.ComponentModel.DataAnnotations;
using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Categories.Models;

public record CreateCategoryDto(
    [property: Required, StringLength(250)] string Name,
    [property: Required, StringLength(2000)] string Description,
    [property: Required, EnumDataType(typeof(CategoryType))] CategoryType Type);
