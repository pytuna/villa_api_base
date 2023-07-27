using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

#nullable disable
namespace VillaApi.Entities;

public class ModelAppContext : IdentityDbContext<ApplicationUser>{
    private readonly ILoggerFactory _loggerFactory;  
    public ModelAppContext(DbContextOptions<ModelAppContext> options, ILoggerFactory loggerFactory) : base(options){
        _loggerFactory = loggerFactory;
    }
    public DbSet<Villa> Villas { get; set; }
    public DbSet<VillaNumber> VillaNumbers { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLoggerFactory(_loggerFactory);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Villa>(entity =>{
            entity.HasIndex(e => e.Name).IsUnique();
        });

        foreach(var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if(tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6));
            }
        }
    }
}