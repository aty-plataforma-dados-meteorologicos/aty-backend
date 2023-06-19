using System.ComponentModel.DataAnnotations.Schema;

namespace AtyBackend.Domain.Entities
{
    [NotMapped]
    public class WeatherDataFlux
    {
        public int WeatherStationId { get; set; }
        public int SensorId { get; set; }
        public string TypeTag { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public List<MeasurementFlux> Measurements { get; set; }
    }
}
