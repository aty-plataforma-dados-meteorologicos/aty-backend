using AutoMapper;
using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using AtyBackend.Application.ViewModels;
using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using AtyBackend.Infrastructure.Data.Repositories;


namespace AtyBackend.Application.Services;

public class WeatherStationService : IWeatherStationService
{
    private readonly IWeatherStationRepository _weatherStationRepository;
    private readonly IMapper _mapper;

    public WeatherStationService(IWeatherStationRepository weatherStationRepository,
        IMapper mapper)
    {
        _weatherStationRepository = weatherStationRepository;
        _mapper = mapper;
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


    public async Task<WeatherStationAuthenticationDTO> GetWeatherStationAuthentication(int? id)
    {
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


}
