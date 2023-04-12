using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AtyBackend.Application.ViewModels;
using AtyBackend.Application.Services;
using AtyBackend.Domain.Entities;
using AtyBackend.API.Helpers;

namespace AtyBackend.API.Controllers;

//[EnableCors()]
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ExemploController : ControllerBase
{
    private readonly IExemploService _exemploService;

    public ExemploController(IExemploService exemploService)
    {
        _exemploService = exemploService;
    }

    [HttpGet]
    public async Task<ActionResult<Paginated<ExemploDTO>>> GetExemploAsync([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var paginated = new ApiResponsePaginated<ExemploDTO>(pageNumber, pageSize);
        var exemplos = await _exemploService.GetAsync(paginated.PageNumber, paginated.PageSize);
        paginated.AddData(exemplos, Request);

        return paginated.Data is null ? NotFound("Exemplos not found") : Ok(paginated);

        //var pageNumberNotNull = pageNumber is null ? 1 : pageNumber.Value;
        //var pageSizeNotNull = pageSize is null ? 10 : pageSize.Value;
        ////var url = "https://" + Request.Host.ToString() + Request.Path.ToString();
        ////url = Request.IsHttps ? url : url.Replace("https", "http");

        //var exemplos = await _exemploService.GetAsync(pageNumberNotNull, pageSizeNotNull);

        //return exemplos is null ? NotFound("Exemplos not found") : Ok(exemplos);
    }

    [HttpGet("{id:int}", Name = "GetExemploById")]
    public async Task<ActionResult<ExemploDTO>> GetByIdAsync(int? id)
    {
        var exemplo = await _exemploService.GetByIdAsync(id);
        return exemplo is null ? NotFound("Exemplo not found") : Ok(exemplo);
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync(ExemploDTO exemploDto)
    {
        if (exemploDto == null)
        {
            return BadRequest("Exemplo is null");
        }

        exemploDto = await _exemploService.CreateAsync(exemploDto);

        return new CreatedAtRouteResult("GetExemploById", new { id = exemploDto.Id }, exemploDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateAsync(int? id, [FromBody] ExemploDTO exemploDto)
    {
        if (id != exemploDto.Id)
        {
            return BadRequest("Id is different from Exemplo.Id");
        }
        
        var result = await _exemploService.UpdateAsync(exemploDto);
        return (result is not null) ? Ok(result) : NotFound("Exemplo not found");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> RemoveAsync(int id) => await _exemploService.DeleteAsync(id) ? NoContent() : BadRequest("Not deleted");
}
