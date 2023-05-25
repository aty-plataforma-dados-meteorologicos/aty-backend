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
    public async Task<WeatherStationDTO> CreateAsync(WeatherStationDTO dto)
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
        return _mapper.Map<WeatherStationDTO>(weatherStation);
    }

    public async Task<Paginated<WeatherStationView>> GetAsync(int pageNumber, int pageSize)
    {
        var total = await _weatherStationRepository.CountAsync();
        var entities = await _weatherStationRepository.GetAllAsync(pageSize, pageNumber);
        var dtos = _mapper.Map<List<WeatherStationView>>(entities);

        return new Paginated<WeatherStationView>(pageNumber, pageSize, total, dtos);
    }

    public async Task<WeatherStationDTO> GetByIdAsync(int? id)
    {
        var weatherStationEntity = await _weatherStationRepository.GetByIdAsync(id);
        var weatherStation = _mapper.Map<WeatherStationDTO>(weatherStationEntity);
        //var weatherStation = _mapper.Map<WeatherStationView>(weatherStationEntity);

        return weatherStation;
    }

    public async Task<WeatherStationDTO> UpdateAsync(WeatherStationDTO weatherStation)
    {
        weatherStation.UpdateAt = DateTime.UtcNow;
        var weatherStationEntity = _mapper.Map<WeatherStation>(weatherStation);
        var weatherStationUpdated = await _weatherStationRepository.UpdateAsync(weatherStationEntity);

        return _mapper.Map<WeatherStationDTO>(weatherStationUpdated);
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
            userWeatherStationEntity = await _weatherStationUserRepository.UpdateAsync(userWeatherStationEntity);

            return userWeatherStationEntity.IsMaintainer;
        }

        var userDto = new WeatherStationUserDTO
        {
            WeatherStationId = weatherStationUser.WeatherStationId,
            ApplicationUserId = user.Id,
            IsMaintainer = true,
            IsDataAuthorized = false,
            IsFavorite = false
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

    // remove maintener
    public async Task<bool> RemoveMaintainer(int weatherStationId, string maintainerId)
    {
        // tem que ter mais de 1 mantenedor para poder deletar esse
        var totalMaintainers = await _weatherStationUserRepository.CountByConditionAsync(u => u.WeatherStationId == weatherStationId && u.IsMaintainer);
        if (totalMaintainers <= 1 ) throw new Exception("It is not possible to remove the maintainer as the station does not have another maintainer.");

        var weatherStationUserDto = await _weatherStationUserRepository.GetByIdAsync(weatherStationId, maintainerId);
        weatherStationUserDto.IsMaintainer = false;


        var weatherStationUserEntity = _mapper.Map<WeatherStationUser>(weatherStationUserDto);
        weatherStationUserEntity = await _weatherStationUserRepository.UpdateAsync(weatherStationUserEntity);
        
        return !weatherStationUserEntity.IsMaintainer;
    }

    public async Task<Paginated<WeatherStationUserDTO>> GetWeatherStationMaintainers(int weatherStationId, int pageNumber, int pageSize)
    {
        var totalMaintainers = await _weatherStationUserRepository.CountByConditionAsync(u => u.WeatherStationId == weatherStationId && u.IsMaintainer);
        var entitiesMaintainers = await _weatherStationUserRepository.FindByConditionAsync(u => u.WeatherStationId == weatherStationId && u.IsMaintainer, pageNumber, pageSize);
        var dtos = _mapper.Map<List<WeatherStationUserDTO>>(entitiesMaintainers);

        foreach(var dto in dtos)
        {
            var user = await _userManager.FindByIdAsync(dto.ApplicationUserId);
            dto.ApplicationUserEmail = user.Email;
            dto.ApplicationUserName = user.Name;
        }

        return new Paginated<WeatherStationUserDTO>(pageNumber, pageSize, totalMaintainers, dtos);
    }

    public async Task<Paginated<WeatherStationView>> GetMaintainerWeatherStation(string maintainer, int pageNumber, int pageSize)
    {
        var totalWeatherStations = await _weatherStationUserRepository.CountByConditionAsync(u => u.ApplicationUserId == maintainer && u.IsMaintainer);
        var entitiesWeatherStations = await _weatherStationUserRepository.FindByConditionAsync(u => u.ApplicationUserId == maintainer && u.IsMaintainer, pageNumber, pageSize);
        //var weatherStations = _mapper.Map<List<WeatherStationUserDTO>>(entitiesWeatherStations);

        List<WeatherStationView> weatherStationViews = new List<WeatherStationView>();

        if (entitiesWeatherStations is not null)
        {
            var weatherStationIds = entitiesWeatherStations.Select(w => w.WeatherStationId).ToList();
            // get all weather stations by weatherStationIds and add to weatherStationViews using _mapper.Map<WeatherStationView>

            foreach(int id in weatherStationIds)
            {
                var weatherStation = await _weatherStationRepository.GetByIdAsync(id);
                var weatherStationView = _mapper.Map<WeatherStationView>(weatherStation);
                weatherStationViews.Add(weatherStationView);
            }
        }

        var result = new Paginated<WeatherStationView>(pageNumber, pageSize, totalWeatherStations, weatherStationViews);

        return result;
    }

    // addFavorite
    // get favorites by ApplicationUserId
    // get Mantened by ApplicationUserId
    // get dataAuth by ApplicationUserId [futuro, mas deixa implementado]
    // Autoriza dados [futuro, mas deixa implementado]

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

    //public async Task<bool> GetWeatherStationMaintainers(int weatherStationId)

}
