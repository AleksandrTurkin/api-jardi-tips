using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace JardiTips.Infrastructure.Data;

public class EntityDbContext(IConfiguration configuration, DbContextOptions<EntityDbContext> options): DbContext(options)
{
    protected readonly IConfiguration Configuration = configuration;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var assembly = typeof(EntityDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
}

