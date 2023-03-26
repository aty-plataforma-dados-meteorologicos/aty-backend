using AtyBackend.Domain.Entities;
using System.Linq.Expressions;

namespace AtyBackend.Domain.Interfaces;

public interface IExemploRepository
{
    Task<Exemplo> CreateAsync(Exemplo entity);

    Task<int> CountAsync();
    Task<int> CountByConditionAsync(Expression<Func<Exemplo, bool>> expression);
    Task<List<Exemplo>> GetAllAsync();
    Task<List<Exemplo>> GetAllAsync(int pageSize, int pageNumber);
    Task<List<Exemplo>> FindByConditionAsync(Expression<Func<Exemplo, bool>> expression);
    Task<List<Exemplo>> FindByConditionAsync(Expression<Func<Exemplo, bool>> expression, int pageSize, int pageNumber);
    Task<Exemplo> GetByIdAsync(int? id);

    Task<Exemplo> UpdateAsync(Exemplo entity);
    Task<bool> DeleteAsync(Exemplo entity); 
}
