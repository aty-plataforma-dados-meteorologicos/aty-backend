using AutoMapper;
using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using AtyBackend.Application.ViewModels;
using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using AtyBackend.Infrastructure.Data.Repositories;


namespace AtyBackend.Application.Services;

public class SensorService : ISensorService
{
    private readonly ISensorRepository _sensorRepository;
    private readonly IMapper _mapper;

    public SensorService(ISensorRepository sensorRepository,
        IMapper mapper)
    {
        _sensorRepository = sensorRepository;
        _mapper = mapper;
    }
    public async Task<SensorDTO> CreateAsync(SensorDTO dto)
    {
        var sensorEntity = _mapper.Map<Sensor>(dto);
        var sensor = await _sensorRepository.CreateAsync(sensorEntity);

        return _mapper.Map<SensorDTO>(sensor);
    }
    
    public async Task<Paginated<SensorDTO>> GetAsync(int pageNumber, int pageSize)
    {
        var total = await _sensorRepository.CountAsync();
        var entities = await _sensorRepository.GetAllAsync(pageSize, pageNumber);
        var dtos = _mapper.Map<List<SensorDTO>>(entities);

        return new Paginated<SensorDTO>(pageNumber, pageSize, total, dtos);

        //var totalItems = await _sensorRepository.CountAsync();
        //var totalPages = (totalItems > 0 && totalItems < pageSize) ? 1 : TotalPages(totalItems, pageSize);

        //var sensorEntities = await _sensorRepository.GetAllAsync(pageSize, pageNumber);
        //var sensorsDTOs = _mapper.Map<List<SensorDTO>>(sensorEntities);

        //var paginatedResult = new Paginated<SensorDTO>
        //{
        //    PageNumber = pageNumber,
        //    PageSize = pageSize,
        //    TotalPages = totalPages,
        //    TotalItems = totalItems,
        //    Data = sensorsDTOs
        //};



        //return paginatedResult;
    }

    public async Task<SensorDTO> GetByIdAsync(int? id)
    {
        var sensorEntity = await _sensorRepository.GetByIdAsync(id);
        var sensor = _mapper.Map<SensorDTO>(sensorEntity);

        return sensor;
    }
    
    public async Task<SensorDTO> UpdateAsync(SensorDTO sensor)
    {
        var sensorEntity = _mapper.Map<Sensor>(sensor);
        var sensorUpdated = await _sensorRepository.UpdateAsync(sensorEntity);

        return _mapper.Map<SensorDTO>(sensorUpdated);
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var sensor = await _sensorRepository.GetByIdAsync(id);
        if (sensor is null)
        {
            return false;
        }

        return await _sensorRepository.DeleteAsync(sensor);
    }
    
    
}
