using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Domain.Entities
{
    [NotMapped]
    public class WeatherData
    {
        public int WeatherStationId { get; set; }
        public DateTime Timestamp { get; set; }
        public List<Measurement> Measurements { get; set; }
    }
}
