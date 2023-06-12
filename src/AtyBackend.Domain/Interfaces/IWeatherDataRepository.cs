using AtyBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Domain.Interfaces
{
    public interface IWeatherDataRepository
    {
        Task<bool> SaveWeatherDataAsync(WeatherData weatherData);
        Task<List<WeatherData>> GetWeatherDataAsync(int weatherStationId, DateTime startDate, DateTime endDate, int? sensorId);
    }
}
