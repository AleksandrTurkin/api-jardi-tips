using System.ComponentModel.DataAnnotations;

namespace JardiTips.Application.Features.Authentication.Models;

public record RefreshTokenDto(
    [property: Required, StringLength(512)] string RefreshToken);
