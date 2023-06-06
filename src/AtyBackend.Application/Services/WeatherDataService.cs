using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public WeatherDataService(
            IWeatherDataRepository weatherDataRepository,
            IMapper mapper
            )
        {
            _weatherDataRepository = weatherDataRepository;
            _mapper = mapper;

        }

        // alterar para addd por lista List<WeatherDataDto> weatherData
        public async Task<bool> SaveWeatherDataAsync(List<WeatherDataDTO> weatherData)
        {
            // falta auto mapper here   e retornar bool
            var weatherDataToInsert = _mapper.Map<List<WeatherData>>(weatherData);

            // após validar o isnert em test, organizar para fazer isso com um insert apenas que já isnere tudo 
            await _weatherDataRepository.SaveWeatherDataAsync(weatherDataToInsert.First());
            //foreach (var item in weatherDataToInsert)
            //{
            //}
            

            return true;
        }

        public Task<List<WeatherDataDTO>> GetWeatherDataAsync(int weatherStationId, DateTime startDate, DateTime endDate, int? sensorId)
        {
            throw new NotImplementedException();
        }

    }
}
