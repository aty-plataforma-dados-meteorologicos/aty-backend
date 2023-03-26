using AtyBackend.Domain.Interfaces;

namespace AtyBackend.Domain.Entities
{
    public class WeatherStationUser
    {
        public int WeatherStationId { get; set; }
        public WeatherStation WeatherStation { get; set; }

        public int ApplicationUserId { get; set; }
        public IApplicationUser? ApplicationUser { get; set; }

        public bool IsDataAuthorized { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsMaintainer { get; set; }
    }
}
