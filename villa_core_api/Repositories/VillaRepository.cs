using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VillaApi.Interfaces;
using VillaApi.Entities;

namespace VillaApi.Repositories;

public class VillaRepository : Repository<Villa>, IVillaRepository
{
    private readonly ModelAppContext _db;
    public VillaRepository(ModelAppContext db) : base(db)
    {
        _db = db;
    }

    public override async Task UpdateAsync(Villa entity)
    {
        entity.UpdatedAt = DateTime.Now;
        await base.UpdateAsync(entity);
    }
}