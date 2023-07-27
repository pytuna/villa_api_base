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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }


    }
    private static void ConfigureServices(WebApplicationBuilder builder)
    {

        var services = builder.Services;

        services.AddAutoMapper(typeof(MappingConfig));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ModelAppContext>()
            .AddDefaultTokenProviders();

        services
            .AddAuthentication(options=>{
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options=>{
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters(){
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
            });

        services.AddDbContext<ModelAppContext>(options =>
        {
            options.UseMySql(builder.Configuration.GetConnectionString("ModelAppContext"), new MySqlServerVersion(new Version(8, 0, 30)));
        });

        services.AddScoped<IVillaRepository, VillaRepository>();
        services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();

        services.AddScoped<VillaService>();
        services.AddScoped<VillaNumberService>();

        services.AddControllers(options =>
        {
            // options.ReturnHttpNotAcceptable = true;
        })
        .AddNewtonsoftJson(options => {
            
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        });

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