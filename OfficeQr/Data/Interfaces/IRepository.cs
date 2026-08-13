using System.Linq.Expressions;

namespace OfficeQr.Data.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query(bool asNoTracking = true);

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    
}