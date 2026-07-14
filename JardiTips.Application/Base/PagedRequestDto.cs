
using System.ComponentModel.DataAnnotations;

namespace JardiTips.Application.Base;

public record PagedRequestDto(
    string? PageContext,
    [property: Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")] int? Limit
);

