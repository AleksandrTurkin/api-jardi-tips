namespace JardiTips.Domain.Entities;

public class UserEntity : BaseEntity
{
    public string Email { get; set; }

    public string DisplayName { get; set; }

    public string? AvatarUrl { get; set; }

    public string? PasswordHash { get; set; }

    public ICollection<UserLoginEntity> Logins { get; set; } = [];
}
