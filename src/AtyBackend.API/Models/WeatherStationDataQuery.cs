namespace AtyBackend.API.Models
{
    public class WeatherStationDataQuery
    {
        // se isso for null, retorna ultimas 24h
        public DateTime StartDateTime { get; set; }
        public DateTime StopDateDateTime { get; set; }

        // posteriormente adicionar o parametro para intervvalo de tempo entre medidas
    }
}
