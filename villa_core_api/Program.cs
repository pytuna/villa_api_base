using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using VillaApi.Services;
using VillaApi.Entities;
using System.Reflection;
using VillaApi.Config;
using VillaApi.Interfaces;
using VillaApi.Repositories;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace VillaApi;

class Program
{
    static public void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
            ConfigureServices(builder);

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}")
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}")
                .CreateLogger();

            Serilog.ILogger logger = Log.Logger;
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
            builder.Host.UseSerilog();
        }

        var app = builder.Build();
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseSerilogRequestLogging();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseCors("open");

            app.UseEndpoints(enpoints =>
            {
                enpoints.MapControllers();
            });

            app.Run();
        }


    }
    private static void ConfigureServices(WebApplicationBuilder builder)
    {

        var services = builder.Services;

        services.AddAutoMapper(typeof(MappingConfig));

        services.ConfigIdentity(builder);
        
        services.AddDbContext<ModelAppContext>(options =>
        {
            options.UseMySql(builder.Configuration.GetConnectionString("ModelAppContext"), new MySqlServerVersion(new Version(8, 0, 30)));
        });

        /* 
            Add repositories
         */
        {
            services.AddScoped<IVillaRepository, VillaRepository>();
            services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }

        /* 
            Add services
        */
        {
            services.AddScoped<VillaService>();
            services.AddScoped<VillaNumberService>();
        }

        // Hủy tự động validate model
        // services.Configure<ApiBehaviorOptions>(options =>
        // {
        //     options.SuppressModelStateInvalidFilter = true;
        // });

        services
            .AddControllers(options =>
            {
                // options.ReturnHttpNotAcceptable = true;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.ConfigSwagger();


    }
}