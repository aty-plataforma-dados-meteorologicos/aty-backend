using AtyBackend.Application.DTOs;
using AtyBackend.Application.ViewModels;

namespace AtyBackend.Application.Interfaces;

public interface IExemploGenericService
{
    Task<ExemploGenericDTO> CreateAsync(ExemploGenericDTO exemploGeneric);
    Task<Paginated<ExemploGenericDTO>> GetAsync(int pageSize, int pageNumber);
    Task<ExemploGenericDTO> GetByIdAsync(int? id);
    Task<ExemploGenericDTO> UpdateAsync(ExemploGenericDTO exemploGeneric);
    Task<bool> DeleteAsync(int id);
}
