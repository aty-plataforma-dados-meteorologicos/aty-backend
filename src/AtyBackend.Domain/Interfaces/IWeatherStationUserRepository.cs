using AtyBackend.Domain.Entities;
using System.Linq.Expressions;

namespace AtyBackend.Domain.Interfaces;

public interface IWeatherStationUserRepository
{
    Task<WeatherStationUser> CreateAsync(WeatherStationUser entity);
    Task<int> CountAsync();
    Task<int> CountByConditionAsync(Expression<Func<WeatherStationUser, bool>> expression);
    Task<List<WeatherStationUser>> GetAllAsync();
    Task<List<WeatherStationUser>> GetAllAsync(int pageSize, int pageNumber);
    Task<List<WeatherStationUser>> FindByConditionAsync(Expression<Func<WeatherStationUser, bool>> expression);
    Task<List<WeatherStationUser>> FindByConditionAsync(Expression<Func<WeatherStationUser, bool>> expression, int pageSize, int pageNumber);
    Task<WeatherStationUser> GetByIdAsync(int? id);
    Task<WeatherStationUser> UpdateAsync(WeatherStationUser entity);
    Task<bool> DeleteAsync(WeatherStationUser entity); 
}
