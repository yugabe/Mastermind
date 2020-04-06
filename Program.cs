using Mastermind.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading.Tasks;

namespace Mastermind.Api
{
    public static class Program
    {
        public static async Task Main(string[] args) =>
            (await CreateHostBuilder(args)
                .Build()
                .MigrateOrReacreateDatabaseAsync())
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

        private static async Task<IHost> MigrateOrReacreateDatabaseAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MastermindDbContext>();
            var allMigrations = dbContext.Database.GetMigrations().ToHashSet();
            var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync();
            if (appliedMigrations.Any(m => !allMigrations.Contains(m)))
            {
                await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.MigrateAsync();
            }
            else if (allMigrations.Any(m => !appliedMigrations.Contains(m)))
                await dbContext.Database.MigrateAsync();
            return host;
        }
    }
}
