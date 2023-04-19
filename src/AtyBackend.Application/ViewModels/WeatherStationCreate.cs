using AtyBackend.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Application.ViewModels
{
    public class WeatherStationCreate
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
    }
}
