using AutoMapper;
using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using AtyBackend.Application.ViewModels;
using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;


namespace AtyBackend.Application.Services;

public class ExemploService : IExemploService
{
    private readonly IExemploRepository _exemploRepository;
    private readonly IMapper _mapper;

    public ExemploService(IExemploRepository exemploRepository,
        IMapper mapper)
    {
        _exemploRepository = exemploRepository;
        _mapper = mapper;
    }
    public async Task<ExemploDTO> CreateAsync(ExemploDTO exemploDTO)
    {
        var exemploEntity = _mapper.Map<Exemplo>(exemploDTO);
        var exemplo = await _exemploRepository.CreateAsync(exemploEntity);

        return _mapper.Map<ExemploDTO>(exemplo);
    }
    
    public async Task<Paginated<ExemploDTO>> GetAsync(int pageNumber, int pageSize)
    {
        var totalItems = await _exemploRepository.CountAsync();
        var exemploEntities = await _exemploRepository.GetAllAsync(pageSize, pageNumber);
        var exemplosDTOs = _mapper.Map<List<ExemploDTO>>(exemploEntities);

        #region old
        //var paginatedResult = new Paginated<ExemploDTO>(pageNumber, pageSize, totalItems, exemplosDTOs);


        //var totalPages = (totalItems > 0 && totalItems < pageSize) ? 1 : TotalPages(totalItems, pageSize);

        ////var paginatedResult = new Paginated<ExemploDTO>
        ////{
        ////    PageNumber = pageNumber,
        ////    PageSize = pageSize,
        ////    TotalPages = totalPages,
        ////    TotalItems = totalItems,
        ////    Data = exemplosDTOs
        ////};

        //paginatedResult.PreviousPageUrl = HasPreviousPage(paginatedResult) ? GetPageUrl(paginatedResult, url, false) : null;
        //paginatedResult.NextPageUrl = HasNextPage(paginatedResult) ? GetPageUrl(paginatedResult, url) : null;
        #endregion
        return new Paginated<ExemploDTO>(pageNumber, pageSize, totalItems, exemplosDTOs); ;
    }

    public async Task<ExemploDTO> GetByIdAsync(int? id)
    {
        var exemploEntity = await _exemploRepository.GetByIdAsync(id);
        var exemplo = _mapper.Map<ExemploDTO>(exemploEntity);

        return exemplo;
    }
    
    public async Task<ExemploDTO> UpdateAsync(ExemploDTO exemplo)
    {
        var exemploEntity = _mapper.Map<Exemplo>(exemplo);
        var exemploUpdated = await _exemploRepository.UpdateAsync(exemploEntity);

        return _mapper.Map<ExemploDTO>(exemploUpdated);
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var exemplo = await _exemploRepository.GetByIdAsync(id);
        if (exemplo is null)
        {
            return false;
        }

        return await _exemploRepository.DeleteAsync(exemplo);
    }
    
    private static int TotalPages(double totalItems, double pageSize) => (int)Math.Ceiling(totalItems / pageSize);

    //condição em que não tem não tem previous
    //-> pageNumber = 1
    //-> pageNumber > TotalPages
    //-> pageNumber < 1
    private static bool HasNextPage(Paginated<ExemploDTO> result) =>
        !(result.PageNumber == result.TotalPages || result.PageNumber > result.TotalPages || result.PageNumber < 1);

    //condição em que não tem next
    //-> pageNumber = TotalPages
    //-> pageNumber > TotalPages
    //-> pageNumber < 1
    private static bool HasPreviousPage(Paginated<ExemploDTO> result) =>
        !(result.PageNumber == 1 || result.PageNumber > result.TotalPages || result.PageNumber < 1);

    private static string GetPageUrl(Paginated<ExemploDTO> result, string url, bool isNextPage = true) => isNextPage ?
        url + "?pageNumber=" + (result.PageNumber + 1) + "&pageSize=" + result.PageSize :
        url + "?pageNumber=" + (result.PageNumber - 1) + "&pageSize=" + result.PageSize;
}
