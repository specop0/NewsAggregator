using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsAggregator.Database;
using NewsAggregator.Parser;

namespace NewsAggregator;

public class Program
{
    public static void Main(string[] args)
    {
        ILogger? logger = null;
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(swagger =>
        {
            swagger.EnableAnnotations();
        });
        builder.Services.AddScoped<DatabaseService>();
        builder.Services.AddHttpClient();

        var useIntegratedBrowser = builder.Configuration.GetSection("Browser:UseIntegrated").Get<bool>();
        if (useIntegratedBrowser)
        {
            builder.Services.AddScoped<IBrowser, IntegratedBrowser>();
        }
        else
        {
            builder.Services.AddScoped<IBrowser, ExternalBrowser>();
        }

        var app = builder.Build();
        logger = app.Logger;

        var pathBase = app.Configuration.GetValue<string>("PathBase");
        if (!string.IsNullOrEmpty(pathBase))
        {
            app.Logger.LogInformation("PathBase: {}", pathBase);
            app.UsePathBase(pathBase);
        }

        // allow cross origin requests (e.g., from frontend in development)
        var allowedCrossOrigin = app.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        if (allowedCrossOrigin.Any())
        {
            app.Logger.LogInformation("CORS.AllowedOrigins: {}", string.Join(", ", allowedCrossOrigin));
            app.UseCors(cors =>
            {
                cors.AllowAnyHeader();
                cors.AllowAnyMethod();
                cors.AllowCredentials();
                cors.WithOrigins(allowedCrossOrigin);
            });

            app.Logger.LogInformation("Swagger available at: {}{}", pathBase, "/swagger");
            app.UseSwagger();
            app.UseSwaggerUI(swagger =>
            {
                swagger.DisplayOperationId();
            });
        }

        app.UseStaticFiles();
        app.UseRouting();

        app.MapControllerRoute(
            name: "default",
            pattern: "api/{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
