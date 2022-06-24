using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace Summit.Platform.WebHostCustomization
{
    public static class WebHostExtensions
    {
        public static IHost MigrateDbContext<TContext>(this IHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using var scope = webHost.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetService<TContext>();

            var contextName = typeof(TContext).Name;

            try
            {
                logger.LogInformation("Migrating database associated with context {DbContextName}", contextName);

                var policy = Policy
                    .Handle<SqlException>()
                    .WaitAndRetry(10, retryAttempt => TimeSpan.FromSeconds(1)
                        ,(exception, timeSpan) => 
                        {
                            logger.LogInformation("Attempting database migration for {DbContextName}", contextName);
                        });

                policy.Execute(() =>
                {
                    context.Database.Migrate();
                    seeder(context, services);
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", contextName);
            }

            return webHost;
        }
    }
}
