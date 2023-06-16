using AtyBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Domain.Entities
{
    [NotMapped]
    public class Measurement
    {
        public int SensorId { get; set; }
        public int? WeatherStationId { get; set; }
        public double MeasurementValue { get; set; }
        public string? TypeTag { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
