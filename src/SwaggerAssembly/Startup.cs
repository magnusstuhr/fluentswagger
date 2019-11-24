using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SwaggerAssembly.Config;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SwaggerAssembly
{
    public class Startup
    {
        public readonly SwaggerConfig SwaggerConfig;

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            SwaggerConfig = new SwaggerConfig(Configuration, typeof(Startup).Assembly);
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(options =>
            {
                CreateSwaggerDoc(options, SwaggerConfig.VersionName, SwaggerConfig.Title,
                    SwaggerConfig.Description, SwaggerConfig.VersionNumber);

                IncludeComments(options, SwaggerConfig.XmlFileName);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options => { options.SwaggerEndpoint(SwaggerConfig.DocUrl, SwaggerConfig.Description); });

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static void CreateSwaggerDoc(SwaggerGenOptions options, string versionName, string title,
            string description, string version)
        {
            options.SwaggerDoc(versionName, new OpenApiInfo
            {
                Title = title,
                Description = description,
                Version = version
            });
        }

        private static void IncludeComments(SwaggerGenOptions options, string fileName)
        {
            options.IncludeXmlComments(GetXmlCommentsPath(fileName));
        }

        private static string GetXmlCommentsPath(string fileName)
        {
            return $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{fileName}";
        }
    }
}