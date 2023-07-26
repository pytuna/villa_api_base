using System.Linq.Expressions;
using VillaApi.Models;
namespace VillaApi.Interfaces;

public interface IVillaRepository
{
    Task<IEnumerable<Villa>> GetAllVillasAsync(int limit, int offset, Expression<Func<Villa, bool>>? filter = null, Func<IQueryable<Villa>, IOrderedQueryable<Villa>>? orderBy = null, string? includeProperties = null);
    Task<Villa?> GetVillaByIdAsync(Expression<Func<Villa, bool>>? filter, bool tracked = true, string? includeProperties = null);
    Task AddVillaAsync(Villa villa);
    Task UpdateVillaAsync(Villa villa);
    Task DeleteVillaAsync(int id);
    Task SaveAsync();
}
