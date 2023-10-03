using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NotifyMe.Infrastructure.Context;

namespace NotifyMe.IoC.Configuration.DI
{
    public static class PreparingServiceExtensions
    {
        public static void InitializeDatabase(this IApplicationBuilder app, ILogger? logger)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                if (dbContext.Database.CanConnect())
                {
                    logger?.LogInformation("Yes, I've got connected to the DatabaseContext");

                    logger?.LogInformation("Migrations started");
                    //dbContext.Database.EnsureDeleted();
                    //dbContext.Database.EnsureCreated();
                    //dbContext.Database.Migrate();
                    logger?.LogInformation("Migrations finished");
                }
                else
                {
                    logger?.LogInformation("No, I haven't connected to the DatabaseContext");
                }
            }
        }
    }
}
