namespace JardiTips.Domain.Entities;

public class TipEntity : BaseEntity
{
    public string Title { get; set; }

    public string Content { get; set; }

    public Guid CategoryId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public CategoryEntity Category { get; set; }
}
