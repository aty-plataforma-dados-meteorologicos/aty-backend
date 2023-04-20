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

        var weatherStationEntity = _mapper.Map<WeatherStation>(dto);
        var weatherStation = await _weatherStationRepository.CreateAsync(weatherStationEntity);


        // get with include
        //weatherStation = await _weatherStationRepository.GetByIdAsync(weatherStation.Id);

        return _mapper.Map<WeatherStationDTO>(weatherStation);
    }
    
    public async Task<Paginated<WeatherStationDTO>> GetAsync(int pageNumber, int pageSize)
    {
        var total = await _weatherStationRepository.CountAsync();
        var entities = await _weatherStationRepository.GetAllAsync(pageSize, pageNumber);
        var dtos = _mapper.Map<List<WeatherStationDTO>>(entities);

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
    
    
}
