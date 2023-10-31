using AtyBackend.Domain.Enums;
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

        public DataAuthEnum IsDataAuthorized { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsMaintainer { get; set; }
        public bool IsCreator { get; set; }

        public bool IsDeleted { get; set; }
    }
}
