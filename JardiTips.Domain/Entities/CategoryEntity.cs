using JardiTips.Domain.Enums;

namespace JardiTips.Domain.Entities;

public class CategoryEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Guid? OwnerUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public CategoryType Type { get; set; }
}