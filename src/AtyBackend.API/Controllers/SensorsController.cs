
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
[Authorize(Roles = "Admin,Manager")]
[Route("api/[controller]")]
[ApiController]
public class SensorsController : ControllerBase
{
    private readonly ISensorService _exemploService;

    public SensorsController(ISensorService exemploService)
    {
        _exemploService = exemploService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponsePaginated<SensorDTO>>> GetSensorAsync([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        // isso aqui ser processado em um contrutor
        //var pageNumberNotNull = pageNumber is null ? 1 : pageNumber.Value;
        //var pageSizeNotNull = pageSize is null ? 10 : pageSize.Value;


        // passar request para o ApiResponsePaginated.AddData
        //var url = "https://" + Request.Host.ToString() + Request.Path.ToString();
        //url = Request.IsHttps ? url : url.Replace("https", "http");

        // aqio
        var paginated = new ApiResponsePaginated<SensorDTO>(pageNumber, pageSize);
        var exemplos = await _exemploService.GetAsync(paginated.PageNumber, paginated.PageSize);
        paginated.AddData(exemplos, Request);

        return paginated.Data is null ? NotFound("Sensors not found") : Ok(paginated);
    }

    [HttpGet("{id:int}", Name = "GetSensorById")]
    public async Task<ActionResult<SensorDTO>> GetByIdAsync(int? id)
    {
        var exemplo = await _exemploService.GetByIdAsync(id);
        return exemplo is null ? NotFound("Sensor not found") : Ok(exemplo);
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync(SensorDTO exemploDto)
    {
        if (exemploDto == null)
        {
            return BadRequest("Sensor is null");
        }

        exemploDto = await _exemploService.CreateAsync(exemploDto);

        return new CreatedAtRouteResult("GetSensorById", new { id = exemploDto.Id }, exemploDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateAsync(int? id, [FromBody] SensorDTO exemploDto)
    {
        if (id != exemploDto.Id)
        {
            return BadRequest("Id is different from Sensor.Id");
        }
        
        var result = await _exemploService.UpdateAsync(exemploDto);
        return (result is not null) ? Ok(result) : NotFound("Sensor not found");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> RemoveAsync(int id) => await _exemploService.DeleteAsync(id) ? NoContent() : BadRequest("Not deleted");
}
