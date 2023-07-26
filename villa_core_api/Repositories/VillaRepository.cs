using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VillaApi.Interfaces;
using VillaApi.Models;

namespace VillaApi.Repositories;

public class VillaRepository : IVillaRepository
{
    private readonly ModelAppContext _context;
    public VillaRepository(ModelAppContext context)
    {
        _context = context;
    }

    public async Task AddVillaAsync(Villa villa)
    {
        var villaCreated = await _context.AddAsync(villa);
        await SaveAsync();
    }

    public async Task DeleteVillaAsync(int id)
    {
        _context.Remove(new Villa() { Id = id });
        await SaveAsync();
    }

    public async Task UpdateVillaAsync(Villa villa)
    {
        _context.Update(villa);
        await SaveAsync();
    }

    public async Task<IEnumerable<Villa>> GetAllVillasAsync(int limit, int offset ,Expression<Func<Villa, bool>>? filter = null, Func<IQueryable<Villa>, IOrderedQueryable<Villa>>? orderBy = null, string? includeProperties = null)
    {
        IQueryable<Villa> query = _context.Villas.Skip(limit * offset).Take(limit);

        if(filter != null) query = query.Where(filter);

        if(orderBy != null) query = orderBy(query);

        return await query.ToListAsync();
    }

    public async Task<Villa?> GetVillaByIdAsync(Expression<Func<Villa, bool>>? filter, bool tracked = true, string? includeProperties = null)
    {
        IQueryable<Villa> query = _context.Villas;

        if(!tracked) query = query.AsNoTracking();
        
        if(filter != null) query = query.Where(filter);

        return await query.FirstOrDefaultAsync();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task<IEnumerable<Villa>> GetAllVillasAsync(Expression<Func<Villa, bool>>? filter = null, Func<IQueryable<Villa>, IOrderedQueryable<Villa>>? orderBy = null, string? includeProperties = null)
    {
        throw new NotImplementedException();
    }

    
}