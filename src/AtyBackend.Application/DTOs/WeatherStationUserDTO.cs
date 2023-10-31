using AtyBackend.Domain.Enums;
using AtyBackend.Infrastructure.Data.Identity;
using System.Text.Json.Serialization;

namespace AtyBackend.Application.DTOs
{
    public class WeatherStationUserDTO : EntityDTO
    {
        public int WeatherStationId { get; set; }
        [JsonIgnore]
        public WeatherStationDTO WeatherStation { get; set; }

        public string ApplicationUserId { get; set; }

        public string? ApplicationUserName { get; set; }
        public string? ApplicationUserEmail { get; set; }

        public DataAuthEnum? IsDataAuthorized { get; set; }
        public bool? IsFavorite { get; set; }
        public bool? IsMaintainer { get; set; }
        public bool? IsCreator { get; set; }
    }
}
