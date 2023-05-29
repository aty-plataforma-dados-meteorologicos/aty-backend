using System.Text.Json.Serialization;

namespace AtyBackend.Application.DTOs
{
    public class WeatherStationView
    {
        public int Id { get; set; }
        public string? PublicId { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public double AltitudeMSL { get; set; }
        //public bool IsPrivate { get; set; }
        public bool IsEnabled { get; set; }

        public bool? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public List<PartnerDTO>? Partners { get; set; }
        public List<SensorDTO> Sensors { get; set; }
    }
}
