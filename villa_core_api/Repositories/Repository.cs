using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VillaApi.Interfaces;
using VillaApi.Entities;

namespace VillaApi.Repositories;

public abstract class Repository<T> : IRepository<T> where T : class
{
    private readonly ModelAppContext _db;
    internal DbSet<T> _dbSet;

    public Repository(ModelAppContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await SaveAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await SaveAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(int limit, int offset, Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet.Skip(limit * offset).Take(limit);

        if(includeProperties != null){
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        if (filter != null) query = query.Where(filter);

        if (orderBy != null) query = orderBy(query);

        return await query.ToListAsync();
    }

    public virtual async Task<T?> GetOneAsync(Expression<Func<T, bool>>? filter, bool tracked = true, string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet;

        if(includeProperties != null){
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        if (!tracked) query = query.AsNoTracking();

        if (filter != null) query = query.Where(filter);

        return await query.FirstOrDefaultAsync();
    }

    public virtual async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }

}