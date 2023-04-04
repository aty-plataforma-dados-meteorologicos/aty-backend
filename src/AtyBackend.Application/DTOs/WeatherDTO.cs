using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Application.DTOs
{
    public class WeatherDTO
    {
        public DateTime Time { get; set; }
        public string TagWeatherStationId { get; set; }  
        public string TagSensorId { get; set; }
        public string Measurement { get; set; }

        public double? ValueDouble { get; set; }
    }
}
