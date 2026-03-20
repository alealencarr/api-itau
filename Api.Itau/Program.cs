using Api.Itau;
using API.Extensions;
using Application.Itau;
using Infra.Itau;
using Serilog;

Log.Logger = LogExtensions.ConfigureLog();

try
{
    Log.Information("Iniciando aplicação...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddApplication()
                    .AddInfrastructure()
                    .AddPresentation(builder.Configuration);
                   

    var app = builder.Build();

    await app.InitializeApp(Log.Logger);

    app.RegisterPipeline();
    app.MapGet("/", () => Results.Ok("Running"));
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação terminou inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}