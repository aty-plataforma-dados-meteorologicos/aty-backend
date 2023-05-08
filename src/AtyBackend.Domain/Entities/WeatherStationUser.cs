using AtyBackend.Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtyBackend.Domain.Entities
{
    public class WeatherStationUser
    {
        public int WeatherStationId { get; set; }

        [NotMapped]
        public WeatherStation? WeatherStation { get; set; }

        public string ApplicationUserId { get; set; }

        [NotMapped]
        public IApplicationUser? ApplicationUser { get; set; }

        public bool IsDataAuthorized { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsMaintainer { get; set; }
    }
}
