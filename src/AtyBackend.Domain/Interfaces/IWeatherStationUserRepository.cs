using AtyBackend.Domain.Entities;
using System.Linq.Expressions;

namespace AtyBackend.Domain.Interfaces;

public interface IWeatherStationUserRepository
{
    Task<WeatherStationUser> CreateAsync(WeatherStationUser entity);
    Task<int> CountAsync();
    Task<int> CountByConditionAsync(Expression<Func<WeatherStationUser, bool>> expression);
    Task<List<WeatherStationUser>> GetAllAsync();
    Task<List<WeatherStationUser>> GetAllAsync(int pageNumber, int pageSize);
    Task<List<WeatherStationUser>> FindByConditionAsync(Expression<Func<WeatherStationUser, bool>> expression);
    Task<List<WeatherStationUser>> FindByConditionAsync(Expression<Func<WeatherStationUser, bool>> expression, int pageNumber, int pageSize);
    Task<WeatherStationUser> GetByIdAsync(int weatherStationId, string applicationUserId);
    Task<WeatherStationUser> UpdateAsync(WeatherStationUser entity);
    Task<bool> DeleteAsync(WeatherStationUser entity); 
}
