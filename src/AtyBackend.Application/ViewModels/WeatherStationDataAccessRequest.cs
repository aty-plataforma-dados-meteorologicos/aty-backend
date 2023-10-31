using AtyBackend.Application.DTOs;
using AtyBackend.Domain.Enums;

namespace AtyBackend.Application.ViewModels
{
    public class WeatherStationDataAccessRequest
    {
        public int WeatherStationId { get; set; }
        public string UserEmail { get; set; }
        public string UserId { get; set; }
        public DataAuthEnum RequestStatus { get; set; }
    }
}
