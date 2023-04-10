using AtyBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AtyBackend.Application.DTOs
{
    public class WeatherStationSensorDTO
    {
        public int WeatherStationId { get; set; }
        public WeatherStationDTO WeatherStation { get; set; }

        public int SensorId { get; set; }
        public SensorDTO Sensor { get; set; }
    }
}
