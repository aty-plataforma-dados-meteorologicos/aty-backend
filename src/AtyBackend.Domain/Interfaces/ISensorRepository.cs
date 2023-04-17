using AtyBackend.Domain.Entities;
using System.Linq.Expressions;

namespace AtyBackend.Domain.Interfaces;

public interface ISensorRepository
{
    Task<Sensor> CreateAsync(Sensor entity);
    Task<int> CountAsync();
    Task<int> CountByConditionAsync(Expression<Func<Sensor, bool>> expression);
    Task<List<Sensor>> GetAllAsync();
    Task<List<Sensor>> GetAllAsync(int pageSize, int pageNumber);
    Task<List<Sensor>> FindByConditionAsync(Expression<Func<Sensor, bool>> expression);
    Task<List<Sensor>> FindByConditionAsync(Expression<Func<Sensor, bool>> expression, int pageSize, int pageNumber);
    Task<Sensor> GetByIdAsync(int? id);
    Task<Sensor> UpdateAsync(Sensor entity);
    Task<bool> DeleteAsync(Sensor entity); 
}
