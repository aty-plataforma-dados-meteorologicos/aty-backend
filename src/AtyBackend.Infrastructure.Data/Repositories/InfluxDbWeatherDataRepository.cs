﻿using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace AtyBackend.Infrastructure.Data.Repositories
{
    public class InfluxDbWeatherDataRepository : IWeatherDataRepository
    {
        private readonly string _influxToken = "5VPrX6lStbpFdZNMVDRH_LKPeAyIPqJrLx9t_su03d-BCJsY_6JECIyDmrEk-lujUSEGThJhrXItv5KD1OG7Sw==";
        private readonly string _influxUrl = "http://localhost:8086";
        private readonly string _bucket = "test-bucket";
        private readonly string _org = "aty";
        private readonly InfluxDBClient _influxDBClient;

        public InfluxDbWeatherDataRepository()
        {
            var options = new InfluxDBClientOptions.Builder()
                .Url(_influxUrl)
                .AuthenticateToken(_influxToken)
                .Build();

            _influxDBClient = InfluxDBClientFactory.Create(options);
        }

        public async Task<bool> SaveWeatherDataAsync(WeatherData weatherData)
        {
            try
            {
                using (var writeApi = _influxDBClient.GetWriteApi())
                {
                    List<PointData> points = new();

                    foreach (var measurement in weatherData.Measurements)
                    {
                        var point = PointData.Measurement(measurement.TypeTag)
                            .Tag("WeatherStationId", weatherData.WeatherStationId.ToString())
                            .Tag("SensorId", measurement.SensorId.ToString())
                            .Field("value", measurement.MeasurementValue)
                            .Timestamp(weatherData.Timestamp, WritePrecision.Ms);

                        points.Add(point);
                    }

                    writeApi.WritePoints(points, _bucket, _org);

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<WeatherDataFlux> GetWeatherDataAsync(int weatherStationId, int sensorId, DateTime start, DateTime stop)
        {
            try
            {

                var flux = $"from(bucket:\"{_bucket}\") " +
                                $"|> range(start: {ToInfluxDateTime(start)}, stop: {ToInfluxDateTime(stop)}) " +
                                $"|> filter(fn: (r) => " +
                                $"r.WeatherStationId == \"{weatherStationId}\" and " +
                                $"r.SensorId == \"{sensorId}\")";
                var queryApi = _influxDBClient.GetQueryApi();
                var fluxTables = await _influxDBClient.GetQueryApi().QueryAsync(flux, _org);

                List<MeasurementFlux> measurements = new();

                fluxTables.ForEach(fluxTable =>
                {
                    var fluxRecords = fluxTable.Records;
                    fluxRecords.ForEach(fluxRecord =>
                    {
                        MeasurementFlux measurement = new MeasurementFlux();
                        measurement.TimeStamp = fluxRecord.GetTime().GetValueOrDefault().ToDateTimeUtc();
                        measurement.MeasurementValue = Convert.ToDouble(fluxRecord.GetValue());

                        measurements.Add(measurement);
                    });
                });

                WeatherDataFlux weatherDataFlux = new WeatherDataFlux
                {
                    WeatherStationId = weatherStationId,
                    SensorId = sensorId,
                    TypeTag = fluxTables[0].Records[0].GetMeasurement(),
                    Start = start,
                    Stop = stop,
                    Measurements = measurements
                };

                return weatherDataFlux;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private string ToInfluxDateTime(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
        }
    }
}
