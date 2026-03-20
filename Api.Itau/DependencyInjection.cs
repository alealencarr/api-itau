using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;
using Serilog;
using System.Text.Json.Serialization;

namespace Api.Itau;
public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog();

        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
        services.AddControllers();
        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        services.AddOpenApi("v1");
        services.AddHttpContextAccessor();
        services.AddSwaggerGen(x =>
        {

            x.CustomSchemaIds(n => n.FullName);
            x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Alexandre Alencar - Desafio", Version = "v1", Description = "API - Produtos e Pedidos" });
        });

        return services;
    }


    public static void RegisterPipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseExceptionHandler(o => { });

        if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
        {
            app.AddApiDocumentation();

        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors(builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
        app.MapControllers();
        app.MapFallbackToFile("index.html");
    }
    public static void AddApiDocumentation(this WebApplication app)
    {
        app.AddSwagger();
        app.AddScalar();
    }

    public static void AddSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    public static void AddScalar(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Tech Challenge - Alexandre Alencar RM364893")
            .AddPreferredSecuritySchemes("Bearer")
            .AddHttpAuthentication("Bearer", options =>
            {
                options.Token = "teste";
            });
        });
    }
}

