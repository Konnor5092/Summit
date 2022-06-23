using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Summit.Platform.WebHostCustomization
{
    public static class WebHostExtensions
    {
        public static void MigrateDbContext(this IHost webHost)
        {

        }

        //public static void MigrateDbContext<TContext>(this IHost webHost) where TContext : DbContext
        //{

        //}
    }
}
