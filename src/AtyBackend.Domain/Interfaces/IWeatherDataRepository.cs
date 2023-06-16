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
        Task<WeatherDataFlux> GetWeatherDataAsync(int weatherStationId, int sensorId, DateTime start, DateTime stop);
    }
}
