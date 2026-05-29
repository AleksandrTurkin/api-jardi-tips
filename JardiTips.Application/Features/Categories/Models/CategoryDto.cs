using JardiTips.Domain;

namespace JardiTips.Application.Features.Categories.Models
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public CategoryType Type { get; set; }
    }
}
