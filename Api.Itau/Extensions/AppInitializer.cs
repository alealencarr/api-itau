using Infra.Itau.DbContexts;
using Infra.Itau.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;
internal static class AppInitializer
{
    internal static async Task<IApplicationBuilder> InitializeApp(this WebApplication app, Serilog.ILogger logger)
    {
        await app.InitializeDatabase(logger);
        return app;
    }

    private static async Task<IApplicationBuilder> InitializeDatabase(this IApplicationBuilder app, Serilog.ILogger logger)
    {
        logger.Information("Iniciando inicialização do banco de dados...");

        using var serviceScope = app.ApplicationServices.CreateScope();

        var seeder = serviceScope.ServiceProvider.GetRequiredService<DataSeeder>();
        var appDbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

        await appDbContext.Database.EnsureCreatedAsync(); 
        await seeder.Initialize();

        logger.Information("Banco de dados inicializado com sucesso.");

        return await Task.FromResult(app);
    }
}