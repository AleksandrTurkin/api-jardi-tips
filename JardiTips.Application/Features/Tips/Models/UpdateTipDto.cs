using System.ComponentModel.DataAnnotations;

namespace JardiTips.Application.Features.Tips.Models;

public record UpdateTipDto(
    [property: Required, StringLength(250)] string Title,
    [property: Required] string Content);
