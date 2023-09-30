using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System.Runtime.Intrinsics.X86;

namespace AtyBackend.Infrastructure.Data.Repositories
{
    public class InfluxDbWeatherDataRepository : IWeatherDataRepository
    {
        private readonly string _influxToken = "elKJWIjP34cLzWxvdrdKH01cd5I7pr-dlicwiEkvQply02ggjxQiHnKaH5lDHsFVEp22cw0UhDSEAr3cnN5YZg==";
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

        public async Task<WeatherDataFlux> GetWeatherDataAsync(int weatherStationId, int sensorId, DateTime start, DateTime stop, string? window)
        {
            try
            {

                var flux = $"from(bucket:\"{_bucket}\") " +
                                $"|> range(start: {ToInfluxDateTime(start)}, stop: {ToInfluxDateTime(stop)}) " +
                                $"|> filter(fn: (r) => " +
                                $"r.WeatherStationId == \"{weatherStationId}\" and " +
                                $"r.SensorId == \"{sensorId}\")";
                    
                if (window is not null && IsValidWindow(window))
                {
                    flux = flux +
                                $"|> aggregateWindow(every: {window}, fn: mean, createEmpty: false)" +
                                $"|> yield(name: \"mean\")";
                }

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

        private static bool IsValidWindow(string? window)
        {
            if (string.IsNullOrEmpty(window))
                return false;

            // Verificar o tamanho mínimo e máximo
            if (window.Length < 2 || window.Length > 5)
                return false;

            // Verificar se começa com dígitos numéricos
            if (!char.IsDigit(window[0]))
                return false;

            // Verificar se termina com uma letra válida (m, h ou d)
            char lastChar = window[window.Length - 1];
            if (lastChar != 'm' && lastChar != 'h' && lastChar != 'd')
                return false;

            // Verificar se os caracteres intermediários são dígitos numéricos
            for (int i = 1; i < window.Length - 1; i++)
            {
                if (!char.IsDigit(window[i]))
                    return false;
            }

            // Verificar o valor numérico dentro do intervalo permitido
            int numericValue = int.Parse(window.Substring(0, window.Length - 1));
            if (numericValue < 1 || numericValue > 9999)
                return false;

            return true;
        }

        private string ToInfluxDateTime(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
        }
    }
}
