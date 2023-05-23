using AtyBackend.Application.DTOs;
using AtyBackend.Application.ViewModels;

namespace AtyBackend.Application.Interfaces;

public interface IWeatherStationService
{
    Task<WeatherStationDTO> CreateAsync(WeatherStationDTO exemploGeneric);
    Task<Paginated<WeatherStationView>> GetAsync(int pageSize, int pageNumber);
    Task<WeatherStationDTO> GetByIdAsync(int? id);
    Task<WeatherStationDTO> UpdateAsync(WeatherStationDTO exemploGeneric);
    Task<bool> DeleteAsync(int id);

    Task<WeatherStationAuthenticationDTO> GetWeatherStationAuthentication(int weatherStationId);

    Task<bool> AddMaintainer(WeatherStationIdUserId weatherStationUser);
    Task<bool> IsAdminManagerMainteiner(int weatherStationId, string userEmail);
    Task<bool> RemoveMaintainer(int weatherStationId, string maintainerId);
    Task<Paginated<WeatherStationUserDTO>> GetWeatherStationMaintainers(int weatherStationId, int pageNumber, int pageSize);
    Task<Paginated<WeatherStationView>> GetMaintainerWeatherStation(string maintainer, int pageNumber, int pageSize);
}
