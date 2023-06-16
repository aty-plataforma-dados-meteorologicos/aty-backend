using AtyBackend.Domain.Entities;
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
using AtyBackend.Domain.Enums;

namespace AtyBackend.Infrastructure.Data.Repositories
{
    public class InfluxDbWeatherDataRepository : IWeatherDataRepository
    {
        //private readonly InfluxDBClient _influxDbClient;
        //private const string DatabaseName = "weather_data";
        private readonly string Token = "5VPrX6lStbpFdZNMVDRH_LKPeAyIPqJrLx9t_su03d-BCJsY_6JECIyDmrEk-lujUSEGThJhrXItv5KD1OG7Sw==";
        private readonly string InfluxUrl = "http://localhost:8086";
        private readonly string _bucket = "test-bucket";
        private readonly string _org = "aty";

        //public InfluxDbWeatherDataRepository()
        //{
        //    // Configure a conexão com o InfluxDB
        //    var options = new InfluxDBClientOptions.Builder()
        //        .Url("http://localhost:8086") // substitua pelo endereço do seu banco de dados InfluxDB
        //        .AuthenticateToken("5VPrX6lStbpFdZNMVDRH_LKPeAyIPqJrLx9t_su03d-BCJsY_6JECIyDmrEk-lujUSEGThJhrXItv5KD1OG7Sw==") // substitua pelo seu token de autenticação
        //        .Build();

        //    _influxDbClient = InfluxDBClientFactory.Create(options);
        //}

        public async Task<bool> SaveWeatherDataAsync(WeatherData weatherData)
        {
            try
            {

                using var client = new InfluxDBClient(InfluxUrl, Token);

                //
                // Write Data
                //
                using (var writeApi = client.GetWriteApi())
                {
                    List<PointData> points = new();

                    foreach (var measurement in weatherData.Measurements)
                    {
                        // write
                        var point = PointData.Measurement(measurement.TypeTag)
                            .Tag("WeatherStationId", weatherData.WeatherStationId.ToString())
                            .Tag("SensorId", measurement.SensorId.ToString())
                            .Field("value", measurement.MeasurementValue)
                            .Timestamp(weatherData.Timestamp, WritePrecision.Ms);

                        points.Add(point);

                        //writeApi.WritePoint(point, "bucket_name", "org_id");

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

            #region navigation aid
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

        public async Task<WeatherDataFlux> GetWeatherDataAsync(int weatherStationId, int sensorId, DateTime start, DateTime stop)
        {
            using var client = new InfluxDBClient(InfluxUrl, Token);

            var flux = $"from(bucket:\"{_bucket}\") " +
                            $"|> range(start: {ToInfluxDateTime(start)}, stop: {ToInfluxDateTime(stop)}) " +
                            $"|> filter(fn: (r) => " +
                            $"r.WeatherStationId == \"{weatherStationId}\" and " +
                            $"r.SensorId == \"{sensorId}\")";

            var queryApi = client.GetQueryApi();
            var fluxTables = await client.GetQueryApi().QueryAsync(flux, _org);
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

            #region old test
            ////fluxTables.ForEach(fluxTable =>
            ////{
            ////    var fluxRecords = fluxTable.Records;
            ////    fluxRecords.ForEach(fluxRecord =>
            ////    {
            ////        Console.WriteLine($"{fluxRecord.GetTime()}: {fluxRecord.GetValue()}");
            ////    });
            ////});

            //// AQUI ARRUMAR
            //var weatherDataList = new List<WeatherData>();

            //foreach (var fluxTable in fluxTables)
            //{
            //    var fluxRecords = fluxTable.Records;

            //    foreach (var fluxRecord in fluxRecords)
            //    {
            //        var weatherData = new WeatherData();

            //        weatherData.WeatherStationId = weatherStationId;
            //        weatherData.Timestamp = fluxRecord.GetTime().GetValueOrDefault().ToDateTimeUtc();
            //        weatherData.Measurements = new List<Measurement>();

            //        //record.GetValue().ToString()

            //        foreach (var field in fluxRecord.Values)
            //        {
            //            Measurement measurement = new Measurement();

            //            measurement.SensorId = Convert.ToInt32(fluxRecord.GetValueByKey("SensorId"));
            //            measurement.TypeTag = fluxRecord.GetMeasurement().ToString();
            //            measurement.MeasurementValue = Convert.ToDouble(field.Value);

            //            weatherData.Measurements.Add(measurement);
            //        }

            //        weatherDataList.Add(weatherData);
            //    }
            //}

            //return weatherDataList;



            //var queryApi = _influxDbClient.GetQueryApiAsync();

            //var query = from weather in _influxDbClient.GetQueryable<WeatherData>("weather")
            //            where weather.WeatherStationId == weatherStationId &&
            //                  weather.Timestamp >= startDate &&
            //                  weather.Timestamp <= endDate &&
            //                  (sensorId == null || weather.Measurements.Any(m => m.SensorId == sensorId))
            //            select weather;

            //var result = await queryApi.QueryAsync(query.ToQueryString(), DatabaseName);

            //var weatherDataList = result.Select(r => r.ToObject<WeatherDataDto>()).ToList();

            //return weatherDataList;

            //throw new NotImplementedException();
            #endregion
        }

        private string ToInfluxDateTime(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
        }
    }
}
