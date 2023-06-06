
using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AtyBackend.Application.ViewModels;
using AtyBackend.Application.Services;
using AtyBackend.Domain.Entities;
using AtyBackend.API.Helpers;
using AutoMapper;
using System.Security.Claims;
using AtyBackend.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using AtyBackend.API.Models;
using AtyBackend.Domain.Model;
using System.Net;

namespace AtyBackend.API.Controllers;

//[EnableCors()]
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class WeatherStationsController : ControllerBase
{
    private readonly IWeatherStationService _weatherStationService;
    private readonly WeatherDataService _weatherStationDataService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public WeatherStationsController(
        IWeatherStationService weatherStationService,
        WeatherDataService weatherStationDataService,
        IMapper mapper, UserManager<ApplicationUser> userManager
        )
    {
        _weatherStationService = weatherStationService;
        _weatherStationDataService = weatherStationDataService;
        _userManager = userManager;
        _mapper = mapper;
    }

    #region CRUD Weather Station
    [HttpGet]
    public async Task<ActionResult<ApiResponsePaginated<WeatherStationView>>> GetWeatherStationAsync([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var paginated = new ApiResponsePaginated<WeatherStationView>(pageNumber, pageSize);
        var weatherStations = await _weatherStationService.GetAsync(paginated.PageNumber, paginated.PageSize);
        paginated.AddData(weatherStations, Request);

        return paginated.Data.Count() < 1 ? NotFound("Empty page") : paginated.TotalItems < 1 ? NotFound("Weather Stations not found") : Ok(paginated);
    }

    [HttpGet("{weatherStationId:int}", Name = "GetWeatherStationById")]
    public async Task<ActionResult<WeatherStationView>> GetByIdAsync(int? weatherStationId)
    {
        var weatherStation = await _weatherStationService.GetByIdAsync(weatherStationId);
        return weatherStation is null ? NotFound("Weather Station not found") : Ok(weatherStation);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpGet("{weatherStationId:int}/Admin", Name = "GetWeatherStationByIdMaintainer")]
    public async Task<ActionResult<WeatherStationDTO>> GetByIdAdminAsync(int weatherStationId)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            if (await _weatherStationService.IsAdminManagerMainteiner(weatherStationId, userEmail))
            {
                var weatherStationDto = await _weatherStationService.GetByIdAsync(weatherStationId);
                return weatherStationDto is null ? NotFound("Weather Station not found") : Ok(weatherStationDto);
            }

            return Unauthorized("Unauthorized");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync(WeatherStationCreate weatherStation)
    {
        try
        {
            if (weatherStation == null) return BadRequest("WeatherStation is null");

            var weatherStationDto = _mapper.Map<WeatherStationDTO>(weatherStation);

            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            var user = await _userManager.FindByEmailAsync(userEmail);
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (role == UserRoles.User)
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
                _userManager.AddToRoleAsync(user, UserRoles.Maintainer).Wait();
            }

            weatherStationDto.WeatherStationUsers = new List<WeatherStationUserDTO>();

            var userDto = new WeatherStationUserDTO
            {
                ApplicationUserId = user.Id,
                IsMaintainer = true,
                IsDataAuthorized = true,
                IsFavorite = false
            };

            weatherStationDto.WeatherStationUsers.Add(userDto);
            var ws = await _weatherStationService.CreateAsync(weatherStationDto);

            return new CreatedAtRouteResult("GetWeatherStationByIdMaintainer", new { weatherStationId = ws.Id }, ws);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpPut("{weatherStationId:int}")]
    public async Task<ActionResult> UpdateAsync(int weatherStationId, [FromBody] WeatherStationDTO weatherStationDto)
    {
        if (weatherStationId != weatherStationDto.Id) return BadRequest("WeatherStationId is different from WeatherStation.Id");

        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            if (await _weatherStationService.IsAdminManagerMainteiner(weatherStationId, userEmail))
            {
                var result = await _weatherStationService.UpdateAsync(weatherStationDto);
                return (result is not null) ? Ok(result) : NotFound("WeatherStation not found");
            }

            return Unauthorized("Unauthorized");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpDelete("{weatherStationId:int}")]
    public async Task<ActionResult> RemoveAsync(int weatherStationId)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            if (await _weatherStationService.IsAdminManagerMainteiner(weatherStationId, userEmail))
            {
                return await _weatherStationService.DeleteAsync(weatherStationId) ? NoContent() : BadRequest("Not deleted");
            }

            return Unauthorized("Unauthorized");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion

    #region Maintainers
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpPost("{weatherStationId:int}/Maintainers")]
    public async Task<ActionResult> AddMaintainer(int weatherStationId, [FromBody] WeatherStationIdUserId weatherStationUser)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            if (await _weatherStationService.IsAdminManagerMainteiner(weatherStationId, userEmail))
            {
                return await _weatherStationService.AddMaintainer(weatherStationUser) ? Ok() : BadRequest("Maintainer not added");
            }

            return Unauthorized("Unauthorized");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpGet("{weatherStationId:int}/Maintainers")]
    public async Task<ActionResult<ApiResponsePaginated<WeatherStationUserDTO>>> GetWeatherStationMaintainers(int weatherStationId, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var paginated = new ApiResponsePaginated<WeatherStationUserDTO>(pageNumber, pageSize);
        var maintainers = await _weatherStationService.GetWeatherStationMaintainers(weatherStationId, paginated.PageNumber, paginated.PageSize);
        paginated.AddData(maintainers, Request);

        return paginated.Data.Count() < 1 ? NotFound("Empty page") : paginated.TotalItems < 1 ? NotFound("Maintainers not found") : Ok(paginated);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpGet("Maintainers")]
    public async Task<ActionResult<ApiResponsePaginated<WeatherStationView>>> GetMaintainerWeatherStations([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

        var paginated = new ApiResponsePaginated<WeatherStationView>(pageNumber, pageSize);
        var maintainers = await _weatherStationService.GetMaintainerWeatherStation(userEmail, paginated.PageNumber, paginated.PageSize);
        paginated.AddData(maintainers, Request);

        return paginated.Data.Count() < 1 ? NotFound("Empty page") : paginated.TotalItems < 1 ? NotFound("Maintainers not found") : Ok(paginated);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpDelete("{weatherStationId:int}/Maintainers/{maintainerId}")]
    public async Task<ActionResult> RemoveMaintainer(int weatherStationId, string maintainerId)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");
            if (await _weatherStationService.IsAdminManagerMainteiner(weatherStationId, userEmail))
            {
                return await _weatherStationService.RemoveMaintainer(weatherStationId, maintainerId) ? NoContent() : BadRequest("Not deleted");
            }
            return Unauthorized("Unauthorized");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion

    #region Favorites
    [Authorize]
    [HttpPost("{weatherStationId:int}/Favorites")]
    public async Task<ActionResult> Favorite(int weatherStationId)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            return await _weatherStationService.Favorite(
                new WeatherStationIdUserId { UserEmail = userEmail, WeatherStationId = weatherStationId }
                ) ? Ok() : BadRequest("Maintainer not added");
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [Authorize]
    [HttpGet("Favorites")]
    public async Task<ActionResult<ApiResponsePaginated<WeatherStationView>>> GetFavorites([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            var paginated = new ApiResponsePaginated<WeatherStationView>(pageNumber, pageSize);
            var weatherStations = await _weatherStationService.GetFavorites(userEmail, paginated.PageNumber, paginated.PageSize);
            paginated.AddData(weatherStations, Request);

            return paginated.Data.Count() < 1 ? NotFound("Empty page") : paginated.TotalItems < 1 ? NotFound("The user does not have any favorite weather stations.") : Ok(paginated);
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [Authorize]
    [HttpDelete("{weatherStationId:int}/Favorites")]
    public async Task<ActionResult> RemoveFavorite(int weatherStationId)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            return await _weatherStationService.RemoveFavorite(
                new WeatherStationIdUserId { UserEmail = userEmail, WeatherStationId = weatherStationId }
                ) ? NoContent() : BadRequest("Not deleted");
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }
    #endregion

    #region Data
    // Aqui devo implementar a recepção [post] e busca por dados [get]
    //[Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpPost("{weatherStationId:int}/Data")]
    public async Task<ActionResult> AddData(int weatherStationId, [FromBody] List<WeatherDataDTO> data)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            if (await _weatherStationService.IsAdminManagerMainteiner(weatherStationId, userEmail))
            {
                return await _weatherStationDataService.SaveWeatherDataAsync(data) ? Ok() : BadRequest("Data not added");
            }

            return Unauthorized("Unauthorized");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{weatherStationId:int}/Data")]
    public async Task<IActionResult> GetWeatherData(int weatherStationId, DateTime startDate, DateTime endDate, int? sensorId)
    {
        var weatherData = await _weatherStationDataService.GetWeatherDataAsync(weatherStationId, startDate, endDate, sensorId);

        if (weatherData == null || weatherData.Count == 0)
        {
            return NotFound();
        }

        return Ok(weatherData);
    }
    //public async Task<ActionResult<List<WeatherDataDto>>> GetData(int weatherStationId, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    //{
    //    var paginated = new ApiResponsePaginated<WeatherStationUserDTO>(pageNumber, pageSize);
    //    var maintainers = await _weatherStationService.GetData(weatherStationId, paginated.PageNumber, paginated.PageSize);
    //    paginated.AddData(maintainers, Request);

    //    return paginated.Data.Count() < 1 ? NotFound("Empty page") : paginated.TotalItems < 1 ? NotFound("Maintainers not found") : Ok(paginated);
    //}

    // by sensor id, {weatherStationId:int}/Data/Sensors/{sensorId:int}

    //[Authorize]


    #endregion


    #region old codes
    // esse aqui não sei se manteremos [acho que acaba e removemos token com a ideia de o post ser vinculado ao usuario]

    //[Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    //[HttpGet("{weatherStationId:int}/Authentication", Name = "GetWeatherStationAuthentication")]
    //public async Task<ActionResult<WeatherStationAuthenticationDTO>> GetWeatherStationAuthentication(int weatherStationId)
    //{
    //    try
    //    {
    //        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    //        if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

    //        if (await _weatherStationService.IsAdminManagerMainteiner(weatherStationId, userEmail))
    //        {
    //            // reset 
    //        }

    //        return Unauthorized("User Unauthorized");
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //    // se o usuário é Maintainer, ver se o usuário que está fazendo a requisição é mantenedor da estação
    //    // implementar  
    //    //através do bearer token, saber se o usuário pode ter acesso a isso
    //    //Quem poderá acessar: mantenedor da estação ou manager / admin da plataforma;

    //    // Se o email não estiver presente no token, retorna um erro


    //    //// find user to get id
    //    //var user = await _userManager.FindByEmailAsync(userEmail);
    //    //var roles = await _userManager.GetRolesAsync(user);
    //    //var role = roles.FirstOrDefault();

    //    //if (role == UserRoles.User)
    //    //{
    //    //    await _userManager.RemoveFromRolesAsync(user, roles);
    //    //    _userManager.AddToRoleAsync(user, UserRoles.Maintainer).Wait();
    //    //}



    //    //var weatherStation = await _weatherStationService.GetWeatherStationAuthentication(id, userEmail);
    //    //return weatherStation is null ? NotFound("WeatherStation not found") : Ok(weatherStation);
    //}
    #endregion

}
