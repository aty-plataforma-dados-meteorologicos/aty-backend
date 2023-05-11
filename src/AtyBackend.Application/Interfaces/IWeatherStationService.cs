using AtyBackend.Application.DTOs;
using AtyBackend.Application.ViewModels;

namespace AtyBackend.Application.Interfaces;

public interface IWeatherStationService
{
    Task<WeatherStationDTO> CreateAsync(WeatherStationDTO exemploGeneric);
    Task<Paginated<WeatherStationDTO>> GetAsync(int pageSize, int pageNumber);
    Task<WeatherStationDTO> GetByIdAsync(int? id);
    Task<WeatherStationDTO> UpdateAsync(WeatherStationDTO exemploGeneric);
    Task<bool> DeleteAsync(int id);

    Task<WeatherStationAuthenticationDTO> GetWeatherStationAuthentication(int? id);

    Task<bool> AddMaintainer(WeatherStationIdUserId weatherStationUser);
    Task<bool> IsMainteiner(int weatherStationId, string userEmail);
}
