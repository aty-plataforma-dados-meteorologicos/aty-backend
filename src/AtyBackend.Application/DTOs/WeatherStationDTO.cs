using System.Text.Json.Serialization;

namespace AtyBackend.Application.DTOs
{
    public class WeatherStationDTO : EntityDTO
    {
        public string? PublicId { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public double AltitudeMSL { get; set; }
        public bool IsPrivate { get; set; }

        public string? Token { get; set; }
        //add isso na documentação
        public bool? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        // -> n
        public List<PartnerDTO>? Partners { get; set; }
        //n - n
        public List<SensorDTO> Sensors { get; set; }
        // n - n

        [JsonIgnore]
        public List<WeatherStationUserDTO>? WeatherStationUsers { get; set; }
    }
}
