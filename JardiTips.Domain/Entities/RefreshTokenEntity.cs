namespace JardiTips.Domain.Entities;

public class RefreshTokenEntity : BaseEntity
{
    public Guid UserId { get; set; }

    public string Token { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime ExpiresAt { get; set; }
}
