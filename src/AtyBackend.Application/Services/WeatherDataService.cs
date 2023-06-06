using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using AtyBackend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Application.Services
{
    public class WeatherDataService : IWeatherDataService
    {
        private readonly IWeatherDataRepository _weatherDataRepository;

        public WeatherDataService(IWeatherDataRepository weatherDataRepository)
        {
            _weatherDataRepository = weatherDataRepository;
        }

        // alterar para addd por lista List<WeatherDataDto> weatherData
        public async Task<bool> SaveWeatherDataAsync(WeatherDataDTO weatherData)
        {
            // falta auto mapper here   e retornar bool
            await _weatherDataRepository.SaveWeatherDataAsync(weatherData);
        }

        public Task<List<WeatherDataDTO>> GetWeatherDataAsync(int weatherStationId, DateTime startDate, DateTime endDate, int? sensorId)
        {
            throw new NotImplementedException();
        }

    }
}
