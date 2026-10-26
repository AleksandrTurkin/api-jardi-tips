using JardiTips.Application.Features.Categories.Models;
using JardiTips.Domain.Entities;
using System.Linq.Expressions;

namespace JardiTips.Application.Features.Categories
{
    internal static class CategoryProjection
    {
        public static Expression<Func<CategoryEntity, CategoryDto>> ToDto(Guid? userId) => category =>
            new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Type = category.Type,
                TipsCount = category.Tips.Count(),
                LikesCount = category.Likes.Count(),
                IsLiked = userId.HasValue && category.Likes.Any(like => like.UserId == userId.Value),
                CoverImageUrl = category.CoverImageUrl,
                UpdatedAt = category.UpdatedAt
            };
    }
}
