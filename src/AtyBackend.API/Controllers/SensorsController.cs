
using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AtyBackend.Application.ViewModels;
using AtyBackend.Application.Services;
using AtyBackend.Domain.Entities;

namespace AtyBackend.API.Controllers;

//[EnableCors()]
[Authorize(Roles = "Admin,Manager")]
[Route("api/[controller]")]
[ApiController]
public class SensorController : ControllerBase
{
    private readonly ISensorService _exemploService;

    public SensorController(ISensorService exemploService)
    {
        _exemploService = exemploService;
    }

    [HttpGet]
    public async Task<ActionResult<Paginated<SensorDTO>>> GetSensorAsync([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var pageNumberNotNull = pageNumber is null ? 1 : pageNumber.Value;
        var pageSizeNotNull = pageSize is null ? 10 : pageSize.Value;
        var url = "https://" + Request.Host.ToString() + Request.Path.ToString();
        url = Request.IsHttps ? url : url.Replace("https", "http");

        var exemplos = await _exemploService.GetAsync(pageNumberNotNull, pageSizeNotNull, url);

        return exemplos is null ? NotFound("Sensors not found") : Ok(exemplos);
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
