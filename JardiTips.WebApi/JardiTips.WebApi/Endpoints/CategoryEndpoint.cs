using JardiTips.Application.Base;
using JardiTips.Application.Features.Categories;
using JardiTips.Application.Features.Categories.Models;
using JardiTips.WebApi.Endpoints.Base;
using JardiTips.WebApi.Extensions;

namespace JardiTips.WebApi.Endpoints
{
    public class CategoryEndpoint : IEndpoint
    {
        public void Register(IServiceCollection services)
        {
            services.AddScoped<ICommandHandler<CreateCategoryCommand, Guid>, CreateCategoryCommandHandler>();
            services.AddScoped<IQueryHandler<GetCategoryByIdQuery, CategoryDto>, GetCategoryByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetCategoriesQuery, List<CategoryDto>>, GetCategoriesQueryHandler>();
            services.AddScoped<ICommandHandler<UpdateCategoryCommand, bool>, UpdateCategoryCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteCategoryCommand, bool>, DeleteCategoryCommandHandler>();
        }

        public void Map(IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/categories").WithTags("Category");

            group.MapPostCommand<CreateCategoryCommand, CreateCategoryDto>("", dto => new CreateCategoryCommand(dto));
            group.MapGetByIdQuery<GetCategoryByIdQuery, CategoryDto, Guid>("{id:guid}", id => new GetCategoryByIdQuery(id));
            group.MapGetFilterQuery<GetCategoriesQuery, List<CategoryDto>, CategoriesFilterDto>("", filters => new GetCategoriesQuery(filters));
            group.MapPutCommand<UpdateCategoryCommand, UpdateCategoryDto, Guid>("{id:guid}", (id, dto) => new UpdateCategoryCommand(id, dto));
            group.MapDeleteCommand<DeleteCategoryCommand, Guid>("{id:guid}", id => new DeleteCategoryCommand(id));
        }
    }
}
