using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Enums;
using AtyBackend.Domain.Interfaces;
using AutoMapper;

namespace AtyBackend.Application.Services
{
    public class WeatherDataService : IWeatherDataService
    {
        private readonly IWeatherDataRepository _weatherDataRepository;
        private readonly IWeatherStationRepository _weatherStationRepository;
        private readonly IMapper _mapper;

        public WeatherDataService(
            IWeatherDataRepository weatherDataRepository,
            IWeatherStationRepository weatherStationRepository,
            IMapper mapper
            )
        {
            _weatherDataRepository = weatherDataRepository;
            _weatherStationRepository = weatherStationRepository;
            _mapper = mapper;

        }

        public async Task<bool> SaveWeatherDataAsync(WeatherDataDTO weatherData)
        {
            try
            {
                await InsertData(weatherData);

                return true;
            }
            catch (Exception) { throw; }
        }

        public async Task<bool> SaveWeatherTimeSeriesDataAsync(List<WeatherDataDTO> weatherData)
        {
            try
            {
                // refatorar para otimizar esse insert
                foreach (var item in weatherData)
                {
                    await InsertData(item);
                }

                return true;
            }
            catch (Exception) { throw; }
        }

        public async Task<WeatherDataFluxDTO> GetWeatherDataAsync(int weatherStationId, int sensorId, DateTime start, DateTime end)
        {
            var weatherData = await _weatherDataRepository.GetWeatherDataAsync(weatherStationId, sensorId, start, end);

            WeatherDataFluxDTO weatherDataFluxDTO = _mapper.Map<WeatherDataFluxDTO>(weatherData);

            return weatherDataFluxDTO;
        }

        private async Task InsertData(WeatherDataDTO weatherData)
        {
            var weatherStationEntity = await _weatherStationRepository.GetByIdAsync(weatherData.WeatherStationId);
            var weatherStation = _mapper.Map<WeatherStationView>(weatherStationEntity);
            var sensors = weatherStation.Sensors;

            // validar se a quantidade de sensores é igual a quantidade de medidas
            if (sensors.Count != weatherData.Measurements.Count) throw new Exception("The number of measurements cannot be different from the number of sensors");

            // validar se os ids dos sensores são iguais aos ids das medidas
            foreach (var m in weatherData.Measurements)
            {
                var sensor = sensors.Find(s => s.Id == m.SensorId);
                m.MeasurementTypeTag = sensor is not null ? sensor.MeasurementType : throw new Exception("Invalid Measurement. Measurement.SensorId not in WeatherStation.Sensors");
            }

            #region futuramente refatorar para um automapping
            List<Measurement> measurements = new();
            foreach (var item in weatherData.Measurements)
            {
                Measurement measurement = new()
                {
                    SensorId = item.SensorId,
                    MeasurementValue = item.MeasurementValue,
                    TypeTag = WeatherMeasurementsExtensions.GetMeasurementName(item.MeasurementTypeTag)
                };

                measurements.Add(measurement);
            }

            WeatherData weatherDataToInsert = new()
            {
                WeatherStationId = weatherData.WeatherStationId,
                Timestamp = weatherData.Timestamp,
                Measurements = measurements
            };
            #endregion

            await _weatherDataRepository.SaveWeatherDataAsync(weatherDataToInsert);
        }
    }
}
