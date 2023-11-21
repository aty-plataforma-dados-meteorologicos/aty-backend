using AtyBackend.Domain.Entities;
using System.Linq.Expressions;

namespace AtyBackend.Domain.Interfaces;

public interface IWeatherStationRepository
{
    Task<WeatherStation> CreateAsync(WeatherStation entity);
    Task<int> CountAsync();
    Task<int> CountByConditionAsync(Expression<Func<WeatherStation, bool>> expression);
    Task<List<WeatherStation>> GetAllAsync();
    Task<List<WeatherStation>> GetAllAsync(int pageSize, int pageNumber);
    Task<List<WeatherStation>> FindByConditionAsync(Expression<Func<WeatherStation, bool>> expression);
    Task<List<WeatherStation>> FindByConditionAsync(Expression<Func<WeatherStation, bool>> expression, int pageSize, int pageNumber);
    Task<WeatherStation> GetByIdAsync(int? id);
    Task<WeatherStation> UpdateAsync(WeatherStation entity);
    Task<bool> DeleteAsync(WeatherStation entity);
    
    Task<string?> GetPhotoByIdAsync(int weatherStationId);
}
