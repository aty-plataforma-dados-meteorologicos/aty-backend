using AtyBackend.Application.DTOs;
using AtyBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Application.Interfaces
{
    public interface IWeatherDataService
    {
        Task<bool> SaveWeatherDataAsync(WeatherDataDTO data);
        Task<WeatherDataFluxDTO> GetWeatherDataAsync(int weatherStationId, int sensorId, DateTime start, DateTime stop);
        //Task<List<WeatherDataDTO>> GetWeatherDataAsync(int weatherStationId, DateTime start, DateTime stop, int? sensorId);
        //Task<bool> SaveWeatherDataAsync(List<WeatherDataDTO> data);
        // posso separar GetWeatherDataAsync em dois métodos se for melhor para implementar 
    }
}
