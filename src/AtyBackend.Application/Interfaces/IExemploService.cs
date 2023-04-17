using AtyBackend.Application.DTOs;
using AtyBackend.Application.ViewModels;

namespace AtyBackend.Application.Interfaces;

public interface IExemploService
{
    Task<ExemploDTO> CreateAsync(ExemploDTO exemploGeneric);
    Task<Paginated<ExemploDTO>> GetAsync(int pageSize, int pageNumber);
    Task<ExemploDTO> GetByIdAsync(int? id);
    Task<ExemploDTO> UpdateAsync(ExemploDTO exemploGeneric);
    Task<bool> DeleteAsync(int id);
}
