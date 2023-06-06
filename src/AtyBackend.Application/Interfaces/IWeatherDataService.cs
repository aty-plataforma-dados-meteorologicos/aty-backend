﻿using AtyBackend.Application.DTOs;
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
        Task<bool> SaveWeatherDataAsync(WeatherDataDTO weatherData);
        Task<List<WeatherDataDTO>> GetWeatherDataAsync(int weatherStationId, DateTime startDate, DateTime endDate, int? sensorId);
        // posso separar GetWeatherDataAsync em dois métodos se for melhor para implementar 
    }
}