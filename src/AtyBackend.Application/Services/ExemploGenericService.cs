using AutoMapper;
using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using AtyBackend.Application.ViewModels;
using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;

namespace AtyBackend.Application.Services;

public class ExemploGenericService : IExemploGenericService
{
    private readonly IGenericRepository<ExemploGeneric> _genericRepository;
    private readonly IMapper _mapper;

    public ExemploGenericService(IGenericRepository<ExemploGeneric> repository, IMapper mapper)
    {
        _genericRepository = repository;
        _mapper = mapper;
    }

    public async Task<ExemploGenericDTO> CreateAsync(ExemploGenericDTO exemploGenericDTO)
    {
        var exemploGenericEntity = _mapper.Map<ExemploGeneric>(exemploGenericDTO);
        var exemploGeneric = await _genericRepository.CreateAsync(exemploGenericEntity);
        exemploGenericDTO = _mapper.Map<ExemploGenericDTO>(exemploGeneric);

        if (exemploGenericDTO is null)
        {
            return null;
        }

        return exemploGenericDTO;
    }

    public async Task<Paginated<ExemploGenericDTO>> GetAsync(int pageSize, int pageNumber, string url)
    {
        var totalExemploGenerics = await _genericRepository.CountAsync();
        var totalPages = (totalExemploGenerics > 0 && totalExemploGenerics < pageSize) ? 1 : TotalPages(totalExemploGenerics, pageSize);

        var exemploGenericEntities = await _genericRepository.GetAllAsync(pageSize, pageNumber);
        var exemploGenerics = _mapper.Map<IEnumerable<ExemploGenericDTO>>(exemploGenericEntities);

        var paginated = new Paginated<ExemploGenericDTO>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalItems = totalExemploGenerics,
            Data = exemploGenerics
        };

        paginated.PreviousPageUrl = HasPreviousPage(paginated) ? GetPageUrl(paginated, url, false) : null;
        paginated.NextPageUrl = HasNextPage(paginated) ? GetPageUrl(paginated, url, true) : null;

        return paginated;
    }

    public async Task<ExemploGenericDTO> GetByIdAsync(int? id)
    {
        var exemploGenericEntity = await _genericRepository.GetByIdAsync(id);
        var exemploGenericDto = _mapper.Map<ExemploGenericDTO>(exemploGenericEntity);

        return exemploGenericDto;
    }

    public async Task<ExemploGenericDTO> UpdateAsync(ExemploGenericDTO exemploGenericDto)
    {
        var exemploGenericEntity = _mapper.Map<ExemploGeneric>(exemploGenericDto);
        var exemploGeneric = await _genericRepository.UpdateAsync(exemploGenericEntity);
        //exemploGenericDto = _mapper.Map<ExemploGenericDTO>(exemploGeneric);

        //return await Task.FromResult(exemploGenericDto);
        return _mapper.Map<ExemploGenericDTO>(exemploGeneric);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var exemplo = await _genericRepository.GetByIdAsync(id);
        if (exemplo is null)
        {
            return false;
        }
        
        return await _genericRepository.DeleteAsync(exemplo);
    }

    private static int TotalPages(double totalItems, double pageSize) => (int)Math.Ceiling(totalItems / pageSize);

    //condição em que não tem não tem previous
    //-> pageNumber = 1
    //-> pageNumber > TotalPages
    //-> pageNumber < 1
    private static bool HasNextPage(Paginated<ExemploGenericDTO> result) =>
        !(result.PageNumber == result.TotalPages || result.PageNumber > result.TotalPages || result.PageNumber < 1);

    //condição em que não tem next
    //-> pageNumber = TotalPages
    //-> pageNumber > TotalPages
    //-> pageNumber < 1
    private static bool HasPreviousPage(Paginated<ExemploGenericDTO> result) =>
        !(result.PageNumber == 1 || result.PageNumber > result.TotalPages || result.PageNumber < 1);

    private static string GetPageUrl(Paginated<ExemploGenericDTO> result, string url, bool isNextPage = true) => isNextPage ?
        url + "?pageNumber=" + (result.PageNumber + 1) + "&pageSize=" + result.PageSize :
        url + "?pageNumber=" + (result.PageNumber - 1) + "&pageSize=" + result.PageSize;
}
