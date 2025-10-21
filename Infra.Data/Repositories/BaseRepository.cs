using System.Linq.Expressions;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories;
public abstract class BaseRepository<T>(IUnitOfWork unitOfWork) : IBaseRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _dbSet = unitOfWork.GetContext().Set<T>();
    public IQueryable<T> GetAll() => _dbSet.AsQueryable();
    public IQueryable<T> Get(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate).AsQueryable();
    public Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate) => Get(predicate).ToListAsync();
    public async Task<T> GetAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) throw new ArgumentException("Entity not found");
        return entity;
    }
    public Task<List<T>> GetNoTrackingAsync(Expression<Func<T, bool>> predicate) => Get(predicate).AsNoTracking().ToListAsync();
    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => _dbSet.AnyAsync(predicate);
    public Task SaveChangesAsync() => unitOfWork.GetContext().SaveChangesAsync();
    public async Task<T> InsertAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) throw new ArgumentException("Entity not found");
        await DeleteAsync(entity);
    }
    public async Task DeleteAsync(T entity)
    {
        if (unitOfWork.GetContext().Entry(entity).State == EntityState.Detached)
            _dbSet.Attach(entity);
        _dbSet.Remove(entity);
        await SaveChangesAsync();
    }
}
