using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Infrastructure.Data.Repositories
{
    internal class InfluxDbWeatherDataRepository : IWeatherDataRepository
    {
        private readonly InfluxDBClient _influxDbClient;
        private const string DatabaseName = "weather_data";

        public InfluxDbWeatherDataRepository()
        {
            // Configure a conexão com o InfluxDB
            var options = new InfluxDBClientOptions.Builder()
                .Url("http://localhost:8086") // substitua pelo endereço do seu banco de dados InfluxDB
                .AuthenticateToken("your-token") // substitua pelo seu token de autenticação
                .Build();

            _influxDbClient = InfluxDBClientFactory.Create(options);
        }

        public async Task SaveWeatherDataAsync(WeatherData weatherData)
        {
            var point = PointData.Measurement("weather")
                .Tag("stationId", weatherData.WeatherStationId.ToString())
                .Timestamp(weatherData.Timestamp, WritePrecision.Ns);

            foreach (var measurement in weatherData.Measurements)
            {
                point = point.Field($"sensor{measurement.SensorId}", measurement.MeasurementValue);
            }

            await _influxDbClient.GetWriteApiAsync().WritePointAsync(DatabaseName, point);
        }


        public Task<List<WeatherData>> GetWeatherDataAsync(int weatherStationId, DateTime startDate, DateTime endDate, int? sensorId)
        {
            throw new NotImplementedException();
        }

    }
}
