namespace AtyBackend.Domain.Entities
{
    public class WeatherStationSensor
    {
        public int WeatherStationId { get; set; }
        public WeatherStation? WeatherStation { get; set; }

        public int SensorId { get; set; }
        public Sensor? Sensor { get; set; }
    }
}
