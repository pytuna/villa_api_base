using System.Linq.Expressions;
namespace VillaApi.Interfaces;
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(
        int limit,
        int offset,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includeProperties = null
    );
    Task<T?> GetOneAsync(
        Expression<Func<T, bool>>? filter, 
        bool tracked = true, 
        string? includeProperties = null
    );

    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveAsync();
}
