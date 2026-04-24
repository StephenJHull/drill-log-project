using Microsoft.EntityFrameworkCore;
using System.IO.Abstractions;
using WebApp.Data;
using WebApp.Initilization;

namespace WebApp.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();
        }

        public static async Task InitializeIdentityAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var fileSystem = scope.ServiceProvider.GetRequiredService<IFileSystem>();

            var identityIntializer = new IdentityInitializer(fileSystem, app.ApplicationServices);

            await identityIntializer.InitializeAsync();
        }
    }
}
