using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace VillaApi.Config
{
    public static class Swagger
    {
        public static IServiceCollection ConfigSwagger(this IServiceCollection services){

            services.AddEndpointsApiExplorer();
            
            services.AddSwaggerGen((options) =>
            {
                options.SwaggerDoc("v1", new() { Title = "VillaApi", Version = "v1" });
                options.EnableAnnotations();
                System.Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.OperationFilter<SecurityRequirementsOperationFilter>(true, "Bearer");
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme (JWT). Example: \"bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

            });

            return services;
        }
    }
}