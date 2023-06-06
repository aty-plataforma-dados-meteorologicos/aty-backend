namespace AtyBackend.API.Models
{
    public class WeatherStationDataQuery
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateDateTime { get; set; }

        // posteriormente adicionar o parametro para intervvalo de tempo entre medidas
    }
}
