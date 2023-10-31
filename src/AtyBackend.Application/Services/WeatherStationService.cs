using AutoMapper;
using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using AtyBackend.Application.ViewModels;
using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using AtyBackend.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using AtyBackend.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using AtyBackend.Domain.Model;
using AtyBackend.Domain.Enums;
using InfluxDB.Client.Api.Domain;

namespace AtyBackend.Application.Services;

public class WeatherStationService : IWeatherStationService
{
    private readonly IWeatherStationRepository _weatherStationRepository;
    private readonly IWeatherStationUserRepository _weatherStationUserRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public WeatherStationService(IWeatherStationRepository weatherStationRepository,
        IWeatherStationUserRepository weatherStationUserRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _weatherStationRepository = weatherStationRepository;
        _weatherStationUserRepository = weatherStationUserRepository;
        _mapper = mapper;
        _userManager = userManager;
    }
    public async Task<WeatherStationView> CreateAsync(WeatherStationDTO dto)
    {
        dto.CreatedAt = DateTime.UtcNow;
        dto.IsEnabled = true;
        // criar endpoint para gerar token 
        dto.Token = Guid.NewGuid().ToString("N");

        var weatherStationEntity = _mapper.Map<WeatherStation>(dto);
        var weatherStation = await _weatherStationRepository.CreateAsync(weatherStationEntity);
        weatherStation.Token = null;

        // get with include
        //weatherStation = await _weatherStationRepository.GetByIdAsync(weatherStation.Id);
        return _mapper.Map<WeatherStationView>(weatherStation);
    }

    public async Task<Paginated<WeatherStationView>> GetAsync(int pageNumber, int pageSize)
    {
        var total = await _weatherStationRepository.CountAsync();
        var entities = await _weatherStationRepository.GetAllAsync(pageSize, pageNumber);
        var dtos = _mapper.Map<List<WeatherStationView>>(entities);

        return new Paginated<WeatherStationView>(pageNumber, pageSize, total, dtos);
    }

    public async Task<WeatherStationView> GetByIdAsync(int? id)
    {
        var weatherStationEntity = await _weatherStationRepository.GetByIdAsync(id);
        var weatherStation = _mapper.Map<WeatherStationView>(weatherStationEntity);
        //var weatherStation = _mapper.Map<WeatherStationView>(weatherStationEntity);

        return weatherStation;
    }

    public async Task<WeatherStationView> UpdateAsync(WeatherStationDTO weatherStation)
    {
        weatherStation.UpdateAt = DateTime.UtcNow;
        var weatherStationEntity = _mapper.Map<WeatherStation>(weatherStation);
        var weatherStationUpdated = await _weatherStationRepository.UpdateAsync(weatherStationEntity);

        return _mapper.Map<WeatherStationView>(weatherStationUpdated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var weatherStation = await _weatherStationRepository.GetByIdAsync(id);
        if (weatherStation is null)
        {
            return false;
        }

        return await _weatherStationRepository.DeleteAsync(weatherStation);
    }

    /// <summary>
    /// Testarxc 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<WeatherStationAuthenticationDTO> GetWeatherStationAuthentication(int weatherStationId)
    {
        var weatherStation = await _weatherStationRepository.GetByIdAsync(weatherStationId);

        if (weatherStation is null)
        {
            throw new ArgumentNullException(nameof(weatherStation));
        }

        return new WeatherStationAuthenticationDTO
        {
            PublicId = weatherStation.PublicId,
            Token = weatherStation.Token
        };
    }

    public async Task<bool> AddMaintainer(WeatherStationIdUserId weatherStationUser)
    {
        var user = await _userManager.FindByEmailAsync(weatherStationUser.UserEmail);

        var userWeatherStation = await _weatherStationUserRepository.FindByConditionAsync(
            u => u.WeatherStationId == weatherStationUser.WeatherStationId
            && u.ApplicationUserId == user.Id);

        if (userWeatherStation.Any())
        {
            var userWeatherStationEntity = _mapper.Map<WeatherStationUser>(userWeatherStation.FirstOrDefault());

            userWeatherStationEntity.IsMaintainer = true;
            userWeatherStationEntity.IsDataAuthorized = DataAuthEnum.YES;
            userWeatherStationEntity = await _weatherStationUserRepository.UpdateAsync(userWeatherStationEntity);

            return userWeatherStationEntity.IsMaintainer;
        }

        var userDto = new WeatherStationUserDTO
        {
            WeatherStationId = weatherStationUser.WeatherStationId,
            ApplicationUserId = user.Id,
            IsMaintainer = true,
            IsDataAuthorized = DataAuthEnum.YES,
            IsFavorite = false,
            IsCreator = false
        };

        var userEntity = _mapper.Map<WeatherStationUser>(userDto);
        userEntity = await _weatherStationUserRepository.CreateAsync(userEntity);

        if (userEntity.IsMaintainer)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (role == UserRoles.User)
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
                _userManager.AddToRoleAsync(user, UserRoles.Maintainer).Wait();
            }
        }

        return userEntity.IsMaintainer;
    }

    public async Task<bool> RemoveMaintainer(int weatherStationId, string maintainerId)
    {
        var totalMaintainers = await _weatherStationUserRepository.CountByConditionAsync(u => u.WeatherStationId == weatherStationId && u.IsMaintainer);
        if (totalMaintainers <= 1) throw new Exception("It is not possible to remove the maintainer as the station does not have another maintainer.");

        var weatherStationUserDto = await _weatherStationUserRepository.GetByIdAsync(weatherStationId, maintainerId);
        weatherStationUserDto.IsMaintainer = false;

        // se a estação for privada, remove acesso aos dados
        var weatherStation = await _weatherStationRepository.GetByIdAsync(weatherStationId);

        if (weatherStation.IsPrivate)
        {
            weatherStationUserDto.IsDataAuthorized = DataAuthEnum.NO;
        }

        var weatherStationUserEntity = _mapper.Map<WeatherStationUser>(weatherStationUserDto);
        weatherStationUserEntity = await _weatherStationUserRepository.UpdateAsync(weatherStationUserEntity);

        var totalWeatherStations = await _weatherStationUserRepository.CountByConditionAsync(u => u.ApplicationUserId == maintainerId && u.IsMaintainer);

        if (totalWeatherStations == 0)
        {
            var user = await _userManager.FindByIdAsync(maintainerId);
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            if (role == UserRoles.Maintainer)
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
                _userManager.AddToRoleAsync(user, UserRoles.User).Wait();
            }
        }

        return !weatherStationUserEntity.IsMaintainer;
    }

    public async Task<Paginated<WeatherStationUserDTO>> GetWeatherStationMaintainers(int weatherStationId, int pageNumber, int pageSize)
    {
        var totalMaintainers = await _weatherStationUserRepository.CountByConditionAsync(u => u.WeatherStationId == weatherStationId && u.IsMaintainer);
        var entitiesMaintainers = await _weatherStationUserRepository.FindByConditionAsync(u => u.WeatherStationId == weatherStationId && u.IsMaintainer, pageNumber, pageSize);
        var dtos = _mapper.Map<List<WeatherStationUserDTO>>(entitiesMaintainers);

        foreach (var dto in dtos)
        {
            var user = await _userManager.FindByIdAsync(dto.ApplicationUserId);
            dto.ApplicationUserEmail = user.Email;
            dto.ApplicationUserName = user.Name;
        }

        return new Paginated<WeatherStationUserDTO>(pageNumber, pageSize, totalMaintainers, dtos);
    }

    public async Task<Paginated<WeatherStationView>> GetMaintainerWeatherStation(string userEmail, int pageNumber, int pageSize)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);

        var totalWeatherStations = await _weatherStationUserRepository.CountByConditionAsync(u => u.ApplicationUserId == user.Id && u.IsMaintainer);
        var entitiesWeatherStations = await _weatherStationUserRepository.FindByConditionAsync(u => u.ApplicationUserId == user.Id && u.IsMaintainer, pageNumber, pageSize);

        List<WeatherStationView> weatherStationViews = new List<WeatherStationView>();

        if (entitiesWeatherStations is not null)
        {
            var weatherStationIds = entitiesWeatherStations.Select(w => w.WeatherStationId).ToList();
            // get all weather stations by weatherStationIds and add to weatherStationViews using _mapper.Map<WeatherStationView>

            foreach (int id in weatherStationIds)
            {
                var weatherStation = await _weatherStationRepository.GetByIdAsync(id);
                var weatherStationView = _mapper.Map<WeatherStationView>(weatherStation);
                weatherStationViews.Add(weatherStationView);
            }
        }

        var result = new Paginated<WeatherStationView>(pageNumber, pageSize, totalWeatherStations, weatherStationViews);

        return result;
    }

    public async Task<bool> Favorite(WeatherStationIdUserId weatherStationUser)
    {
        var errorMEssage = "It is not possible to favorite a private weather station that you are not data authorized.";
        var user = await _userManager.FindByEmailAsync(weatherStationUser.UserEmail);

        var userWeatherStation = await _weatherStationUserRepository.FindByConditionAsync(
            u => u.WeatherStationId == weatherStationUser.WeatherStationId
            && u.ApplicationUserId == user.Id);

        // get station
        var weatherStation = await _weatherStationRepository.GetByIdAsync(weatherStationUser.WeatherStationId);

        if (userWeatherStation.Any())
        {
            var userWeatherStationEntity = _mapper.Map<WeatherStationUser>(userWeatherStation.FirstOrDefault());

            if (weatherStation.IsPrivate && userWeatherStationEntity.IsDataAuthorized == DataAuthEnum.NO) { throw new Exception(errorMEssage); }


            userWeatherStationEntity.IsFavorite = true;
            userWeatherStationEntity = await _weatherStationUserRepository.UpdateAsync(userWeatherStationEntity);

            return userWeatherStationEntity.IsFavorite;
        }

        if (weatherStation.IsPrivate) { throw new Exception(errorMEssage); }

        var userDto = new WeatherStationUserDTO
        {
            WeatherStationId = weatherStationUser.WeatherStationId,
            ApplicationUserId = user.Id,
            IsMaintainer = false,
            IsDataAuthorized = DataAuthEnum.YES,
            IsFavorite = true,
            IsCreator = false
        };

        var userEntity = _mapper.Map<WeatherStationUser>(userDto);
        userEntity = await _weatherStationUserRepository.CreateAsync(userEntity);

        return userEntity.IsFavorite;
    }
    public async Task<Paginated<WeatherStationView>> GetFavorites(string userEmail, int pageNumber, int pageSize)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);

        var totalWeatherStations = await _weatherStationUserRepository.CountByConditionAsync(u => u.ApplicationUserId == user.Id && u.IsFavorite);
        var entitiesWeatherStations = await _weatherStationUserRepository.FindByConditionAsync(u => u.ApplicationUserId == user.Id && u.IsFavorite, pageNumber, pageSize);

        List<WeatherStationView> weatherStationViews = new();

        if (entitiesWeatherStations is not null)
        {
            var weatherStationIds = entitiesWeatherStations.Select(w => w.WeatherStationId).ToList();

            foreach (int id in weatherStationIds)
            {
                var weatherStation = await _weatherStationRepository.GetByIdAsync(id);
                var weatherStationView = _mapper.Map<WeatherStationView>(weatherStation);
                weatherStationViews.Add(weatherStationView);
            }
        }

        var result = new Paginated<WeatherStationView>(pageNumber, pageSize, totalWeatherStations, weatherStationViews);

        return result;
    }
    public async Task<bool> RemoveFavorite(WeatherStationIdUserId weatherStationUser)
    {
        var user = await _userManager.FindByEmailAsync(weatherStationUser.UserEmail);

        var weatherStationUserDto = await _weatherStationUserRepository.GetByIdAsync(weatherStationUser.WeatherStationId, user.Id) ?? throw new Exception("Favorite not found.");
        weatherStationUserDto.IsFavorite = false;

        var weatherStationUserEntity = _mapper.Map<WeatherStationUser>(weatherStationUserDto);
        weatherStationUserEntity = await _weatherStationUserRepository.UpdateAsync(weatherStationUserEntity);

        return !weatherStationUserEntity.IsFavorite;
    }

    public async Task<bool> IsAdminManagerMainteiner(int weatherStationId, string userEmail)
    {
        // get user
        var user = await _userManager.FindByEmailAsync(userEmail);
        // get user/roles
        var roles = await _userManager.GetRolesAsync(user);

        if (roles.FirstOrDefault() == UserRoles.Maintainer)
        {
            // if role == {UserRoles.Maintainer} -> verifica se é mantenedor daquela estação
            // testar isso aqui ou mudar no GetByIdAsync para FirstOrDefaultAsync
            var ws = await _weatherStationUserRepository.GetByIdAsync(weatherStationId, user.Id);
            return ws is not null && ws.IsMaintainer;
        }

        return roles.FirstOrDefault() == UserRoles.Admin || roles.FirstOrDefault() == UserRoles.Manager;
    }

    public async Task<bool> IsDataAuthorized(int weatherStationId, string userEmail)
    {
        // get user
        var user = await _userManager.FindByEmailAsync(userEmail);
        // get user/roles
        var userWeatherStation = await _weatherStationUserRepository.FindByConditionAsync(
            u => u.WeatherStationId == weatherStationId
            && u.ApplicationUserId == user.Id);


        if (userWeatherStation.Any())
        {
            if (userWeatherStation.FirstOrDefault().IsDataAuthorized == DataAuthEnum.YES) { return true; }
        }

        var weatherStation = await _weatherStationRepository.GetByIdAsync(weatherStationId);

        if (!weatherStation.IsPrivate) { return true; }

        return false;
    }

    public async Task RequestDataAccess(WeatherStationIdUserId weatherStationUser)
    {
        var user = await _userManager.FindByEmailAsync(weatherStationUser.UserEmail);

        var userWeatherStation = await _weatherStationUserRepository.FindByConditionAsync(
           u => u.WeatherStationId == weatherStationUser.WeatherStationId
           && u.ApplicationUserId == user.Id);

        if (!userWeatherStation.Any())
        {
            var userDto = new WeatherStationUserDTO
            {
                WeatherStationId = weatherStationUser.WeatherStationId,
                ApplicationUserId = user.Id,
                IsDataAuthorized = DataAuthEnum.PENDING,
                IsMaintainer = false,
                IsFavorite = false,
                IsCreator = false
            };

            var userEntity = _mapper.Map<WeatherStationUser>(userDto);
            userEntity = await _weatherStationUserRepository.CreateAsync(userEntity);
        }
    }

    public async Task<Paginated<WeatherStationAccessInfo>> GetDataAccessRequest(string userEmail, int pageNumber, int pageSize, DataAuthEnum? filter)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);

        List<WeatherStationUser> userWeatherStation = new();
        List<WeatherStationAccessInfo> data = new();
        int total;

        if (filter is not null)
        {
            userWeatherStation = await _weatherStationUserRepository.FindByConditionAsync(
               u => u.ApplicationUserId == user.Id
               && u.IsDataAuthorized == filter, pageNumber, pageSize);

            total = await _weatherStationUserRepository.CountByConditionAsync(u => u.ApplicationUserId == user.Id && u.IsDataAuthorized == filter);
        }
        else
        {
            userWeatherStation = await _weatherStationUserRepository.FindByConditionAsync(
               u => u.ApplicationUserId == user.Id, pageNumber, pageSize);

            total = await _weatherStationUserRepository.CountByConditionAsync(u => u.ApplicationUserId == user.Id);
        }

        if (userWeatherStation.Any())
        {

            foreach (var wsu in userWeatherStation)
            {
                var ws = await _weatherStationRepository.GetByIdAsync(wsu.WeatherStationId);

                var wsai = _mapper.Map<WeatherStationAccessInfo>(ws);
                wsai.RequestStatus = wsu.IsDataAuthorized;

                data.Add(wsai);
            }

        }

        return new Paginated<WeatherStationAccessInfo>(pageNumber, pageSize, total, data);
    }

    public async Task<Paginated<WeatherStationDataAccessRequest>> GetDataAccessRequest(int weatherStationId, int pageNumber, int pageSize, DataAuthEnum? filter)
    {
        // aqui vou buscar todos as wsu de uma estação

        List<WeatherStationUser> userWeatherStation = new();
        List<WeatherStationDataAccessRequest> data = new();
        int total;

        if (filter is not null)
        {
            userWeatherStation = await _weatherStationUserRepository.FindByConditionAsync(
               u => u.WeatherStationId == weatherStationId
               && u.IsDataAuthorized == filter, pageNumber, pageSize);

            total = await _weatherStationUserRepository.CountByConditionAsync(u => u.WeatherStationId == weatherStationId && u.IsDataAuthorized == filter);
        }
        else
        {
            userWeatherStation = await _weatherStationUserRepository.FindByConditionAsync(
               u => u.WeatherStationId == weatherStationId, pageNumber, pageSize);

            total = await _weatherStationUserRepository.CountByConditionAsync(u => u.WeatherStationId == weatherStationId);
        }

        if (userWeatherStation.Any())
        {

            foreach (var wsu in userWeatherStation)
            {
                var user = await _userManager.FindByIdAsync(wsu.ApplicationUserId);

                var wsdar = new WeatherStationDataAccessRequest()
                {
                    WeatherStationId = wsu.WeatherStationId,
                    RequestStatus = wsu.IsDataAuthorized,
                    UserEmail = user.Email,
                    UserId = user.Id
                };

                data.Add(wsdar);
            }

        }

        return new Paginated<WeatherStationDataAccessRequest>(pageNumber, pageSize, total, data);
    }

    public async Task UpdateDataAccess(WeatherStationIdUserId weatherStationUser, DataAuthEnum newAuth)
    {
        var user = await _userManager.FindByEmailAsync(weatherStationUser.UserEmail);

        var userWeatherStation = await _weatherStationUserRepository.FindByConditionAsync(
           u => u.WeatherStationId == weatherStationUser.WeatherStationId
           && u.ApplicationUserId == user.Id);

        if (!userWeatherStation.Any())
        {
            var wsu = userWeatherStation.FirstOrDefault();

            wsu.IsDataAuthorized = newAuth;

            await _weatherStationUserRepository.UpdateAsync(wsu);
        }
    }
}
