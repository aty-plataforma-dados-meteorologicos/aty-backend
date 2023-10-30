using AtyBackend.Application.DTOs;
using AtyBackend.Application.ViewModels;

namespace AtyBackend.Application.Interfaces;

public interface IWeatherStationService
{
    Task<WeatherStationView> CreateAsync(WeatherStationDTO exemploGeneric);
    Task<Paginated<WeatherStationView>> GetAsync(int pageSize, int pageNumber);
    Task<WeatherStationView> GetByIdAsync(int? id);
    Task<WeatherStationView> UpdateAsync(WeatherStationDTO exemploGeneric);
    Task<bool> DeleteAsync(int id);

    Task<WeatherStationAuthenticationDTO> GetWeatherStationAuthentication(int weatherStationId);

    Task<bool> AddMaintainer(WeatherStationIdUserId weatherStationUser);
    Task<bool> IsAdminManagerMainteiner(int weatherStationId, string userEmail);
    Task<bool> RemoveMaintainer(int weatherStationId, string maintainerId);
    Task<Paginated<WeatherStationUserDTO>> GetWeatherStationMaintainers(int weatherStationId, int pageNumber, int pageSize);
    Task<Paginated<WeatherStationView>> GetMaintainerWeatherStation(string userEmail, int pageNumber, int pageSize);

    Task<bool> Favorite(WeatherStationIdUserId weatherStationUser);
    Task<bool> RemoveFavorite(WeatherStationIdUserId weatherStationUser);
    Task<Paginated<WeatherStationView>> GetFavorites(string userEmail, int pageNumber, int pageSize);

    // interfaces relacionadas com a implementação da autorização de acesso aos dados
    Task<bool> IsDataAuthorized(int weatherStationId, string userEmail);

    //Task<bool> AddData(List<WeatherDataDTO> data);
    //Task<WeatherDataView> GetData();


}
