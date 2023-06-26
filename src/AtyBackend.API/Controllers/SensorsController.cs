
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
public class SensorsController : ControllerBase
{
    private readonly ISensorService _exemploService;

    public SensorsController(ISensorService exemploService)
    {
        _exemploService = exemploService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ApiResponsePaginated<SensorDTO>>> GetSensorAsync([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var paginated = new ApiResponsePaginated<SensorDTO>(pageNumber, pageSize);
        var exemplos = await _exemploService.GetAsync(paginated.PageNumber, paginated.PageSize);
        paginated.AddData(exemplos, Request);

        return paginated.Data.Count() < 1 ? NotFound("Empty page") : paginated.TotalItems < 1 ? NotFound("Sensors not found") : Ok(paginated);
    }

    [Authorize]
    [HttpGet("{id:int}", Name = "GetSensorById")]
    public async Task<ActionResult<SensorDTO>> GetByIdAsync(int? id)
    {
        var exemplo = await _exemploService.GetByIdAsync(id);
        return exemplo is null ? NotFound("Sensor not found") : Ok(exemplo);
    }

    [Authorize(Roles = "Admin,Manager")]
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

    [Authorize(Roles = "Admin,Manager")]
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

    [Authorize(Roles = "Admin,Manager")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> RemoveAsync(int id) => await _exemploService.DeleteAsync(id) ? NoContent() : BadRequest("Not deleted");
}
