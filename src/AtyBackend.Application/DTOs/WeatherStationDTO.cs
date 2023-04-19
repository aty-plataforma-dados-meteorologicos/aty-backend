﻿using System.Text.Json.Serialization;

namespace AtyBackend.Application.DTOs
{
    public class WeatherStationDTO : EntityDTO
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public double AltitudeMSL { get; set; }
        public bool IsPrivate { get; set; }
        public string? Token { get; set; }
        //add isso na documentação
        public bool? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // -> n
        public List<PartnerDTO>? Partners { get; set; }
        //n - n
        public List<SensorDTO> Sensors { get; set; }
        // n - n

        public List<WeatherStationUserDTO>? WeatherStationUsers { get; set; }
    }
}
