using JardiTips.Infrastructure.Data;

namespace JardiTips.WebApi.Extensions
{
    public static class DbExtension
    {
        public static void InitializeDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<EntityDbContext>();
                DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("Program");
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }
    }
}
