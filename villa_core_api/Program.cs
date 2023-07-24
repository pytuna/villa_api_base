using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using VillaApi.Services;
using VillaApi.Models;
using System.Reflection;

namespace VillaApi;

class Program
{
    static public void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
            builder.Host.UseSerilog();
            ConfigureServices(builder);
            var logger = Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}")
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }


    }
    private static void ConfigureServices(WebApplicationBuilder builder)
    {

        var services = builder.Services;

        services.AddDbContext<ModelAppContext>(options =>
        {
            options.UseMySql(builder.Configuration.GetConnectionString("ModelAppContext"), new MySqlServerVersion(new Version(8, 0, 30)));
        });

        services.AddScoped<VillaService>();

        services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;

        }).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen((options) =>
        {
            options.SwaggerDoc("v1", new() { Title = "VillaApi", Version = "v1" });
            options.EnableAnnotations();
            System.Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });


    }
}