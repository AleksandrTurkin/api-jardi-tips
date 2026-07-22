using JardiTips.Application.DataAccess;
using JardiTips.Infrastructure.Data;
using JardiTips.Infrastructure.Data.Repositories;
using JardiTips.WebApi.Extensions;
using Microsoft.EntityFrameworkCore;

namespace JardiTips.WebApi;

public static class AppServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddDbContext<EntityDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("MainConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddJwtAuthentication(builder.Configuration);
    }
}

