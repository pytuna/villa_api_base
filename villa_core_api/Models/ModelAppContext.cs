using Microsoft.EntityFrameworkCore;

#nullable disable
namespace VillaApi.Models;

public class ModelAppContext : DbContext{
    private readonly ILoggerFactory _loggerFactory;  
    public ModelAppContext(DbContextOptions<ModelAppContext> options, ILoggerFactory loggerFactory) : base(options){
        _loggerFactory = loggerFactory;
    }
    
    // Default Logger
    // public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { 
    //     builder.AddFilter(DbLoggerCategory.Query.Name, LogLevel.Information);
    //     builder.AddConsole(); 
    // }); 

    public DbSet<Villa> Villas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLoggerFactory(_loggerFactory);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}