﻿namespace AtyBackend.Domain.Entities
{
    public class WeatherStation : Entity
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public double AltitudeMSL { get; set; }
        public bool IsPrivate { get; set; }
        public string? Token { get; set; }
        //add isso na documentação
        public bool? Status { get; set; }

        // -> n
        public List<Partner>? Partners { get; set; }
        //n - n
        public List<WeatherStationSensor> WeatherStationSensors { get; set; }
        // n - n
        public List<WeatherStationUser>? WeatherStationUsers { get; set; }
    }
}