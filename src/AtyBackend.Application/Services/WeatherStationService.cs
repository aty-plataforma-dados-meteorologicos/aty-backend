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
        // criar endpoint para gerar token 
        dto.Token = Guid.NewGuid().ToString("N");

        var weatherStationEntity = _mapper.Map<WeatherStation>(dto);
        var weatherStation = await _weatherStationRepository.CreateAsync(weatherStationEntity);
        weatherStation.Token = null;

        // get with include
        //weatherStation = await _weatherStationRepository.GetByIdAsync(weatherStation.Id);
        return _mapper.Map<WeatherStationDTO>(weatherStation);
    }

    public async Task<Paginated<WeatherStationDTO>> GetAsync(int pageNumber, int pageSize)
    {
        var total = await _weatherStationRepository.CountAsync();
        var entities = await _weatherStationRepository.GetAllAsync(pageSize, pageNumber);
        var dtos = _mapper.Map<List<WeatherStationDTO>>(entities);

        // foreach dtos, dto.token = null
        dtos.ForEach(d =>
        {
            d.Token = null;
        });

        return new Paginated<WeatherStationDTO>(pageNumber, pageSize, total, dtos);
    }

    public async Task<WeatherStationDTO> GetByIdAsync(int? id)
    {
        var weatherStationEntity = await _weatherStationRepository.GetByIdAsync(id);
        var weatherStation = _mapper.Map<WeatherStationDTO>(weatherStationEntity);

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
    public async Task<WeatherStationAuthenticationDTO> GetWeatherStationAuthentication(int weatherStationId, string userEmail)
    {
        if (await IsAdminManagerMainteiner(weatherStationId, userEmail))
        {
            // retorno token + id 
        }

        var weatherStation = await _weatherStationRepository.GetByIdAsync(id);
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
        var userDto = new WeatherStationUserDTO
        {
            WeatherStationId = weatherStationUser.WeatherStationId,
            ApplicationUserId = weatherStationUser.ApplicationUserId,
            IsMaintainer = true,
            IsDataAuthorized = true,
            IsFavorite = false
        };

        var userEntity = _mapper.Map<WeatherStationUser>(userDto);
        userEntity = await _weatherStationUserRepository.CreateAsync(userEntity);
        weatherStationUser = _mapper.Map<WeatherStationUserDTO>(userEntity);

        return weatherStationUser;
    }

    // remove maintener
    public async Task<bool> RemoveMaintainer(WeatherStationUserDTO weatherStationUser)
    {
        // get WeatherStationUserDTO to update
        var userDto = await _weatherStationUserRepository.get
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
            return ws is not null;
        }

        return roles.FirstOrDefault() == UserRoles.Admin || roles.FirstOrDefault() == UserRoles.Manager;
    }



}
