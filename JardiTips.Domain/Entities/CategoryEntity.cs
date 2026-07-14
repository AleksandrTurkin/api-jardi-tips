using JardiTips.Domain.Enums;

namespace JardiTips.Domain.Entities;

public class CategoryEntity : BaseEntity
{
    public string Name { get; set; }

    public string Description { get; set; }

    public Guid? OwnerUserId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public CategoryType Type { get; set; }
}