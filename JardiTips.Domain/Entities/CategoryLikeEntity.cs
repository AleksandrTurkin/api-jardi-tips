namespace JardiTips.Domain.Entities;

public class CategoryLikeEntity : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public UserEntity User { get; set; }

    public CategoryEntity Category { get; set; }
}
