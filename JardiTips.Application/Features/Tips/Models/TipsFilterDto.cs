using System.ComponentModel.DataAnnotations;
using JardiTips.Application.Base;

namespace JardiTips.Application.Features.Tips.Models;

public record TipsFilterDto(
    string? PageContext,
    int? Limit,
    [property: Required] Guid CategoryId)
    : PagedRequestDto(PageContext, Limit);
