using AtyBackend.Application.DTOs;
using AtyBackend.Domain.Enums;

namespace AtyBackend.Application.ViewModels
{
    public class WeatherStationAccessInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public double AltitudeMSL { get; set; }
        //[JsonIgnore]
        public bool IsPrivate { get; set; }
        //add isso na documentação
        //public bool? Status { get; set; }
        //public DateTime? CreatedAt { get; set; }
        //public DateTime? ModifiedAt { get; set; }

        //// -> n
        //public List<PartnerDTO>? Partners { get; set; }
        ////n - n
        //public List<SensorDTO> Sensors { get; set; }

        public DataAuthEnum RequestStatus { get; set; }
    }
}
