using AtyBackend.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AtyBackend.Application.ViewModels
{
    public class WeatherStationCreate
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public double AltitudeMSL { get; set; }
        //[JsonIgnore]
        public bool IsPrivate { get; set; }
        //add isso na documentação
        public bool? Status { get; set; }
        [RegularExpression(@"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$", ErrorMessage = "PhotoBase64 must be a base64")]
        public string? PhotoBase64 { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // -> n
        public List<PartnerDTO>? Partners { get; set; }
        //n - n
        public List<SensorDTO> Sensors { get; set; }
    }
}
