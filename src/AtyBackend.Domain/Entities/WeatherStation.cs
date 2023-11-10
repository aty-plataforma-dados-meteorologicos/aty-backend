namespace AtyBackend.Domain.Entities
{
    public class WeatherStation : Entity
    {
        public string? PublicId { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public double AltitudeMSL { get; set; }
        public bool IsPrivate { get; set; }
        public string? Token { get; set; }
        //add isso na documentação
        public bool? Status { get; set; }
        public string? PhotoBase64 { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        // -> n
        public List<Partner>? Partners { get; set; }
        //n - n
        public List<WeatherStationSensor> WeatherStationSensors { get; set; }
        // n - n
        public List<WeatherStationUser>? WeatherStationUsers { get; set; }
    }
}
