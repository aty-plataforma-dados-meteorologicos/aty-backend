using AtyBackend.Domain.Entities;
using System.Linq.Expressions;

namespace AtyBackend.Domain.Interfaces;

public interface IGenericRepository<T> where T : Entity
{
    Task<T> CreateAsync(T entity);

    Task<int> CountAsync();
    Task<int> CountByConditionAsync(Expression<Func<T, bool>> expression);
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetAllAsync(int pageSize, int pageNumber);
    Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);
    Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, int pageSize, int pageNumber);
    Task<T> GetByIdAsync(int? id);

    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(T entity);
}
