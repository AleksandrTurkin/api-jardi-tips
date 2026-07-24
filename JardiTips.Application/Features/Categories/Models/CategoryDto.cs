using JardiTips.Domain.Enums;

namespace JardiTips.Application.Features.Categories.Models
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public CategoryType Type { get; set; }
    }
}
