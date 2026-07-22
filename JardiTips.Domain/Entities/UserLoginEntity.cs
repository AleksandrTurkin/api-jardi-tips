namespace JardiTips.Domain.Entities;

public class UserLoginEntity : BaseEntity
{
    public Guid UserId { get; set; }

    public string LoginProvider { get; set; }

    public string ProviderKey { get; set; }

    public UserEntity User { get; set; }
}
