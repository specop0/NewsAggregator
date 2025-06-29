using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsAggregator.Database;
using NewsAggregator.OpenApi;
using NewsAggregator.Parser;
using NewsAggregator.Routes;
using NewsAggregator.UseCases;

namespace NewsAggregator;

public class Program
{
    public static void Main(string[] args)
    {
        ILogger? logger = null;
        var builder = WebApplication.CreateSlimBuilder(args);

        var appSettings = new AppSettings();
        builder.Configuration.Bind(appSettings);

        // Add services to the container.
        builder.Services.AddCors();
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });
        builder.Services.AddOpenApi(options =>
        {
            options.RemoveServers();
        });
        builder.Services.AddScoped<GetNewsHandler>();

        builder.Services.AddHttpClient<DatabaseService>(client =>
        {
            var url = appSettings.Database?.Url ?? string.Empty;
            if (!url.EndsWith("/"))
            {
                url = url + "/";
            }
            client.BaseAddress = new Uri(url);
        });

        var useIntegratedBrowser = appSettings.Browser?.UseIntegrated ?? true;
        if (useIntegratedBrowser)
        {
            builder.Services.AddHttpClient<IBrowser, IntegratedBrowser>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30d);
            });
        }
        else
        {
            builder.Services.AddHttpClient<IBrowser, ExternalBrowser>(client =>
           {
               client.Timeout = TimeSpan.FromSeconds(30d);

               var url = appSettings.Browser?.Url ?? string.Empty;
               if (!url.EndsWith("/"))
               {
                   url = url + "/";
               }
               client.BaseAddress = new Uri(url);
           });
        }

        var app = builder.Build();
        logger = app.Logger;

        var pathBase = appSettings.PathBase;
        if (!string.IsNullOrEmpty(pathBase))
        {
            app.Logger.LogInformation("PathBase: {}", pathBase);
            app.UsePathBase(pathBase);
        }

        // allow cross origin requests (e.g., from frontend in development)
        var allowedCrossOrigin = appSettings.Cors?.AllowedOrigins ?? [];
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
        }

        if (app.Environment.IsDevelopment())
        {
            app.Logger.LogInformation("OpenAPI available at: {}{}", pathBase, "/openapi/v1.json");
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "openapi";
                options.DocumentTitle = "OpenAPI UI";
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });
        }

        app.UseRouting();

        app.UseStaticFiles();

        app.MapNewsRoutes();

        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
