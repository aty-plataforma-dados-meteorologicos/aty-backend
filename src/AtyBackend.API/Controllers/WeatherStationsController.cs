
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
public class WeatherStationController : ControllerBase
{
    private readonly IWeatherStationService _weatherStationService;

    public WeatherStationController(IWeatherStationService weatherStationService)
    {
        _weatherStationService = weatherStationService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponsePaginated<WeatherStationDTO>>> GetWeatherStationAsync([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        // isso aqui ser processado em um contrutor
        //var pageNumberNotNull = pageNumber is null ? 1 : pageNumber.Value;
        //var pageSizeNotNull = pageSize is null ? 10 : pageSize.Value;


        // passar request para o ApiResponsePaginated.AddData
        //var url = "https://" + Request.Host.ToString() + Request.Path.ToString();
        //url = Request.IsHttps ? url : url.Replace("https", "http");

        // aqio
        var paginated = new ApiResponsePaginated<WeatherStationDTO>(pageNumber, pageSize);
        var weatherStations = await _weatherStationService.GetAsync(paginated.PageNumber, paginated.PageSize);
        paginated.AddData(weatherStations, Request);

        return paginated.Data is null ? NotFound("WeatherStations not found") : Ok(paginated);
    }

    [HttpGet("{id:int}", Name = "GetWeatherStationById")]
    public async Task<ActionResult<WeatherStationDTO>> GetByIdAsync(int? id)
    {
        var weatherStation = await _weatherStationService.GetByIdAsync(id);
        return weatherStation is null ? NotFound("WeatherStation not found") : Ok(weatherStation);
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync(WeatherStationDTO weatherStationDto)
    {
        if (weatherStationDto == null)
        {
            return BadRequest("WeatherStation is null");
        }

        weatherStationDto = await _weatherStationService.CreateAsync(weatherStationDto);

        return new CreatedAtRouteResult("GetWeatherStationById", new { id = weatherStationDto.Id }, weatherStationDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateAsync(int? id, [FromBody] WeatherStationDTO weatherStationDto)
    {
        if (id != weatherStationDto.Id)
        {
            return BadRequest("Id is different from WeatherStation.Id");
        }
        
        var result = await _weatherStationService.UpdateAsync(weatherStationDto);
        return (result is not null) ? Ok(result) : NotFound("WeatherStation not found");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> RemoveAsync(int id) => await _weatherStationService.DeleteAsync(id) ? NoContent() : BadRequest("Not deleted");
}
