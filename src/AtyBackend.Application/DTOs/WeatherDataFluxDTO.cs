using AtyBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Application.DTOs
{
    public class WeatherDataFluxDTO
    {
        public int WeatherStationId { get; set; }
        public int SensorId { get; set; }
        public SensorDTO Sensor { get; set; }
        public string TypeTag { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public List<MeasurementFluxDTO> Measurements { get; set; }
    }
}
