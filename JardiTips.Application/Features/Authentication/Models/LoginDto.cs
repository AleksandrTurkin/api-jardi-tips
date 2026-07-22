using System.ComponentModel.DataAnnotations;

namespace JardiTips.Application.Features.Authentication.Models;

public record LoginDto(
    [property: Required, EmailAddress, StringLength(320)] string Email,
    [property: Required, StringLength(200)] string Password);
