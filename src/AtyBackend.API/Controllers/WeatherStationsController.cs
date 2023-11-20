
using AtyBackend.API.Helpers;
using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using AtyBackend.Application.ViewModels;
using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Enums;
using AtyBackend.Domain.Model;
using AtyBackend.Infrastructure.Data.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AtyBackend.API.Controllers;

//[EnableCors()]
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class WeatherStationsController : ControllerBase
{
    private readonly IWeatherStationService _weatherStationService;
    private readonly IWeatherDataService _weatherStationDataService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public WeatherStationsController(
        IWeatherStationService weatherStationService,
        IWeatherDataService weatherStationDataService,
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
                IsDataAuthorized = DataAuthEnum.YES,
                IsFavorite = false,
                IsCreator = true
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
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpPost("{weatherStationId:int}/Data")]
    public async Task<ActionResult> AddData(int weatherStationId, [FromBody] WeatherDataDTO data)
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

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpPost("{weatherStationId:int}/Data/TimeSeries")]
    public async Task<ActionResult> AddDataTS(int weatherStationId, [FromBody] List<WeatherDataDTO> data)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            if (await _weatherStationService.IsAdminManagerMainteiner(weatherStationId, userEmail))
            {
                return await _weatherStationDataService.SaveWeatherTimeSeriesDataAsync(data) ? Ok() : BadRequest("Data not added");
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
    public async Task<IActionResult> GetWeatherData(int weatherStationId,
        [FromQuery] int sensor,
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? stop,
        [FromQuery] string? window
        )
    {
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (userEmail is null) { return BadRequest("O token JWT não contém o email do usuário."); }

        var user = await _userManager.FindByEmailAsync(userEmail);

        if (await _weatherStationService.IsDataAuthorized(weatherStationId, userEmail) == false) { return Unauthorized("Unauthorized"); }

        start ??= DateTime.UtcNow.AddHours(-24);
        stop ??= DateTime.UtcNow;

        var weatherData = await _weatherStationDataService.GetWeatherDataAsync(weatherStationId, sensor, start.Value, stop.Value, window);

        if (weatherData == null || weatherData.Measurements.Count == 0) { return NotFound(); }

        return Ok(weatherData);
    }
    #endregion

    #region DataAuth
    [Authorize]
    [HttpGet("{weatherStationId:int}/RequestDataAccess")]
    public async Task<IActionResult> DataAccessRequest(int weatherStationId)
    {
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (userEmail is null) { return BadRequest("O token JWT não contém o email do usuário."); }

        //var user = await _userManager.FindByEmailAsync(userEmail);

        var weatherStationUser = new WeatherStationIdUserId
        {
            UserEmail = userEmail,
            WeatherStationId = weatherStationId
        };

        await _weatherStationService.RequestDataAccess(weatherStationUser);

        return Ok();
    }

    // usuários ver requests dele com filtro por status
    [Authorize]
    [HttpGet("RetrieveDataAccessRequests")]
    public async Task<ActionResult<ApiResponsePaginated<WeatherStationAccessInfo>>> GetUserDataAccessRequest([FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] DataAuthEnum? status)
    {
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (userEmail is null) { return BadRequest("O token JWT não contém o email do usuário."); }
        var paginated = new ApiResponsePaginated<WeatherStationAccessInfo>(pageNumber, pageSize);

        var weatherStationAccessInfo = await _weatherStationService.GetDataAccessRequest(userEmail, paginated.PageNumber, paginated.PageSize, status);

        paginated.AddData(weatherStationAccessInfo, Request);

        if (status is not null)
        {
            string filter = $"&status={(int)status}";

            paginated.FirstPageUrl = paginated.FirstPageUrl is null ? null : paginated.FirstPageUrl + filter;
            paginated.LastPageUrl = paginated.LastPageUrl is null ? null : paginated.LastPageUrl + filter;
            paginated.PreviousPageUrl = paginated.PreviousPageUrl is null ? null : paginated.PreviousPageUrl + filter;
            paginated.NextPageUrl = paginated.NextPageUrl is null ? null : paginated.NextPageUrl + filter;
        }

        return paginated.Data.Count() < 1 ? NotFound("Empty page") : paginated.TotalItems < 1 ? NotFound("Weather Stations not found") : Ok(paginated);
    }

    // admin ver as requests por estação meteorológica
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpGet("{weatherStationId:int}/DataAccessRequests")]
    public async Task<ActionResult<ApiResponsePaginated<WeatherStationDataAccessRequest>>> GetWeatherStationDataAccessRequest([FromQuery] int? pageNumber, [FromQuery] int? pageSize, int weatherStationId, [FromQuery] DataAuthEnum? status)
    {
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

        if (await _weatherStationService.IsAdminManagerMainteiner(weatherStationId, userEmail))
        {
            var paginated = new ApiResponsePaginated<WeatherStationDataAccessRequest>(pageNumber, pageSize);

            var weatherStationDataAccessRequests = await _weatherStationService.GetDataAccessRequest(weatherStationId, paginated.PageNumber, paginated.PageSize, status);

            paginated.AddData(weatherStationDataAccessRequests, Request);

            if (status is not null)
            {
                string filter = $"&status={(int)status}";

                paginated.FirstPageUrl = paginated.FirstPageUrl is null ? null : paginated.FirstPageUrl + filter;
                paginated.LastPageUrl = paginated.LastPageUrl is null ? null : paginated.LastPageUrl + filter;
                paginated.PreviousPageUrl = paginated.PreviousPageUrl is null ? null : paginated.PreviousPageUrl + filter;
                paginated.NextPageUrl = paginated.NextPageUrl is null ? null : paginated.NextPageUrl + filter;
            }

            return paginated.Data.Count() < 1 ? NotFound("Empty page") : paginated.TotalItems < 1 ? NotFound("Weather Stations not found") : Ok(paginated);
        }

        return Unauthorized("Unauthorized");
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpPut("{weatherStationId:int}/DataAccessRequests/{userId}")]
    public async Task<IActionResult> UpdateDataAccessRequest(int weatherStationId, string userId, [FromQuery] DataAuthEnum newAuth)
    {
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (userEmail is null) { return BadRequest("O token JWT não contém o email do usuário."); }

        if (await _weatherStationService.IsAdminManagerMainteiner(weatherStationId, userEmail))
        {

            return await _weatherStationService.UpdateDataAccess(userId, weatherStationId, newAuth) ? Ok() : NotFound("Data access request not found");
        }

        return Unauthorized("Unauthorized");
    }

    #endregion


    [HttpGet("{weatherStationId:int}/Photo")]
    public async Task<ActionResult<string>> GetWeatherStationPhoto(int weatherStationId)
    {
        var result = await _weatherStationService.GetPhotoByIdAsync(weatherStationId);

        if (result is null)
        {
            return NotFound("Photo not found");
        }

        return result;
    }
}
