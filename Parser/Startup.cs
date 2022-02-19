namespace Parser
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Parser.Database;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddSingleton<Browser>();
            services.AddSingleton<DatabaseService>();
        }

        public void Configure(IApplicationBuilder application, IHostApplicationLifetime applicationLifetime, IWebHostEnvironment environment)
        {
            var pathBase = this.Configuration.Get<ParserConfiguration>().PathBase;
            if (!string.IsNullOrEmpty(pathBase))
            {
                application.UsePathBase(pathBase);
            }

            application.UseMvcWithDefaultRoute();
        }
    }
}