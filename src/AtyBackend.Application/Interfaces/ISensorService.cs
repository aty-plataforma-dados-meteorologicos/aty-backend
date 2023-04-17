using AtyBackend.Application.DTOs;
using AtyBackend.Application.ViewModels;

namespace AtyBackend.Application.Interfaces;

public interface ISensorService
{
    Task<SensorDTO> CreateAsync(SensorDTO exemploGeneric);
    Task<Paginated<SensorDTO>> GetAsync(int pageSize, int pageNumber, string url);
    Task<SensorDTO> GetByIdAsync(int? id);
    Task<SensorDTO> UpdateAsync(SensorDTO exemploGeneric);
    Task<bool> DeleteAsync(int id);
}
