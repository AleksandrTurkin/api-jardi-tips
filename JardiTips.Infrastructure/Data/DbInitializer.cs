namespace JardiTips.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(EntityDbContext context)
        {
            var createResult = context.Database.EnsureCreated();

            if (!createResult)
                return;
        }
    }
}
