﻿using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Writes;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Linq;

using System;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Newtonsoft.Json.Linq;

namespace AtyBackend.Infrastructure.Data.Repositories
{
    internal class InfluxDbWeatherDataRepository : IWeatherDataRepository
    {
        private readonly InfluxDBClient _influxDbClient;
        private const string DatabaseName = "weather_data";
        private readonly string Token = "";
        private readonly string InfluxUrl = "http://localhost:8086";

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
            using var client = new InfluxDBClient(InfluxUrl, Token);

            //
            // Write Data
            //
            using (var writeApi = client.GetWriteApi())
            {
                //
                // Write by Data Point

                var point = PointData.Measurement("temperature")
                    .Tag("location", "west")
                    .Field("value", 55D)
                    .Timestamp(DateTime.UtcNow.AddSeconds(-10), WritePrecision.Ns);

                writeApi.WritePoint(point, "bucket_name", "org_id");
            }

            #region by gpt
            //var point = PointData.Measurement("weather")
            //    .Tag("stationId", weatherData.WeatherStationId.ToString())
            //    .Timestamp(weatherData.Timestamp, WritePrecision.Ns);

            //foreach (var measurement in weatherData.Measurements)
            //{
            //    point = point.Field($"sensor{measurement.SensorId}", measurement.MeasurementValue);
            //}

            //await _influxDbClient.GetWriteApiAsync().WritePointAsync(point, DatabaseName);
            #endregion

        }


        public async Task<List<WeatherData>> GetWeatherDataAsync(int weatherStationId, DateTime startDate, DateTime endDate, int? sensorId)
        {
            var queryApi = _influxDbClient.GetQueryApiAsync();

            var query = from weather in _influxDbClient.GetQueryable<WeatherData>("weather")
                        where weather.WeatherStationId == weatherStationId &&
                              weather.Timestamp >= startDate &&
                              weather.Timestamp <= endDate &&
                              (sensorId == null || weather.Measurements.Any(m => m.SensorId == sensorId))
                        select weather;

            var result = await queryApi.QueryAsync(query.ToQueryString(), DatabaseName);

            var weatherDataList = result.Select(r => r.ToObject<WeatherDataDto>()).ToList();

            return weatherDataList;
        }


    }
}
