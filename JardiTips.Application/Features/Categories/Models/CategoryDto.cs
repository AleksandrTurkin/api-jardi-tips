using JardiTips.Domain.Enums;
using JardiTips.Application.Base;

namespace JardiTips.Application.Features.Categories.Models
{
    public class CategoryDto : IPagedItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public CategoryType Type { get; set; }

        public int TipsCount { get; set; }

        public int LikesCount { get; set; }

        public bool IsLiked { get; set; }

        public string? CoverImageUrl { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
