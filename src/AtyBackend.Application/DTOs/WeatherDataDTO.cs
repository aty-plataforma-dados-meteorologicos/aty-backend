namespace AtyBackend.Application.DTOs
{
    public class WeatherDataDto
    {
        public int WeatherStationId { get; set; }
        public DateTime Timestamp { get; set; }
        public List<MeasurementDTO> Measurements { get; set; }
    }
}
