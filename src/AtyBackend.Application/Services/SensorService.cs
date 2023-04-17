using AutoMapper;
using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using AtyBackend.Application.ViewModels;
using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;


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
    
    public async Task<Paginated<SensorDTO>> GetAsync(int pageNumber, int pageSize, string url)
    {
        var totalItems = await _sensorRepository.CountAsync();
        var totalPages = (totalItems > 0 && totalItems < pageSize) ? 1 : TotalPages(totalItems, pageSize);

        var sensorEntities = await _sensorRepository.GetAllAsync(pageSize, pageNumber);
        var sensorsDTOs = _mapper.Map<IEnumerable<SensorDTO>>(sensorEntities);

        var paginatedResult = new Paginated<SensorDTO>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Data = sensorsDTOs
        };

        paginatedResult.PreviousPageUrl = HasPreviousPage(paginatedResult) ? GetPageUrl(paginatedResult, url, false) : null;
        paginatedResult.NextPageUrl = HasNextPage(paginatedResult) ? GetPageUrl(paginatedResult, url) : null;

        return paginatedResult;
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
    
    private static int TotalPages(double totalItems, double pageSize) => (int)Math.Ceiling(totalItems / pageSize);

    //condição em que não tem não tem previous
    //-> pageNumber = 1
    //-> pageNumber > TotalPages
    //-> pageNumber < 1
    private static bool HasNextPage(Paginated<SensorDTO> result) =>
        !(result.PageNumber == result.TotalPages || result.PageNumber > result.TotalPages || result.PageNumber < 1);

    //condição em que não tem next
    //-> pageNumber = TotalPages
    //-> pageNumber > TotalPages
    //-> pageNumber < 1
    private static bool HasPreviousPage(Paginated<SensorDTO> result) =>
        !(result.PageNumber == 1 || result.PageNumber > result.TotalPages || result.PageNumber < 1);

    private static string GetPageUrl(Paginated<SensorDTO> result, string url, bool isNextPage = true) => isNextPage ?
        url + "?pageNumber=" + (result.PageNumber + 1) + "&pageSize=" + result.PageSize :
        url + "?pageNumber=" + (result.PageNumber - 1) + "&pageSize=" + result.PageSize;
}
