using AtyBackend.Domain.Enums;

namespace AtyBackend.Application.DTOs
{
    public class MeasurementDTO
    {
        public int SensorId { get; set; }
        public WeatherMeasurementsEnum MeasurementTypeTag { get; set; }
        public double MeasurementValue { get; set; }
    }
}
