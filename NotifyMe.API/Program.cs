using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Entities;
using NotifyMe.Infrastructure.Context;
using NotifyMe.Infrastructure.Services;
using NotifyMe.IoC.Configuration.DI;

namespace NotifyMe.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Services
            builder.Services.ConfigureBusinessServices(configuration);

            
            var app = builder.Build();
            var logger = app.Services.GetService<ILogger<Program>>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                InitializeDatabase(services, logger);

                var userManager = services.GetRequiredService<UserManager<User>>();
                var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                Task.Run(() => AdminInitializer.SeedAdminUser(rolesManager, userManager));
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while seeding the database.");
            }

            app.Run();
        }

        private static void InitializeDatabase(IServiceProvider sp, ILogger<Program>? logger)
        {
            using (var scope = sp.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                if (dbContext.Database.CanConnect())
                {
                    logger?.LogDebug($"Yes, I've got connected to the {dbContext.Database.ProviderName}");
                    logger?.LogDebug("Migrations started");
                    dbContext.Database.Migrate();
                }
                else
                    logger?.LogDebug($"No, I haven't connected to the {dbContext.Database.ProviderName}");

            }
        }
    }
}