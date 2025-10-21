using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Interfaces.Repositories;
public interface IBaseRepository<T> where T : BaseEntity
{
    IQueryable<T> GetAll();
    IQueryable<T> Get(Expression<Func<T, bool>> predicate);
    Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate);
    Task<T> GetAsync(Guid id);
    Task<List<T>> GetNoTrackingAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task SaveChangesAsync();
    Task<T> InsertAsync(T entity);
    Task DeleteAsync(Guid id);
    Task DeleteAsync(T entity);
}