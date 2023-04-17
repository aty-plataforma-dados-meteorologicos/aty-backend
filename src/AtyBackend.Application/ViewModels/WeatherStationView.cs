using AtyBackend.Application.DTOs;

namespace AtyBackend.Application.ViewModels
{
    public class WeatherStationView : EntityDTO
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public double AltitudeMSL { get; set; }
        public bool IsPrivate { get; set; }
        public bool? Status { get; set; }
    }
}
