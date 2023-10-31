using AtyBackend.Application.DTOs;
using AtyBackend.Application.ViewModels;
using AtyBackend.Domain.Enums;
using System.Runtime.ConstrainedExecution;

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

    //    -> solicitar acesso a estação
    Task RequestDataAccess(WeatherStationIdUserId weatherStationUser);
    //-> ver estações que solicitei acesso e estão pendente de autorização ou negação
    Task<Paginated<WeatherStationAccessInfo>> GetDataAccessRequest(string userEmail, int pageNumber, int pageSize, DataAuthEnum? filter);
    //-> get solicitações de acesso (aceitar filtro: todas (filtro nulo), pendentes, autorizadas, negadas)
    Task<Paginated<WeatherStationDataAccessRequest>> GetDataAccessRequest(int weatherStationId, int pageNumber, int pageSize, DataAuthEnum? filter);
    //-> autorizar/negar acesso
    Task UpdateDataAccess(WeatherStationIdUserId weatherStationUser, DataAuthEnum newAuth);


}
