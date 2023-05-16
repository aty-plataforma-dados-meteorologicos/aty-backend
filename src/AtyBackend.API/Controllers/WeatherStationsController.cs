
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
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public WeatherStationsController(IWeatherStationService weatherStationService,
        IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _weatherStationService = weatherStationService;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponsePaginated<WeatherStationView>>> GetWeatherStationAsync([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var paginated = new ApiResponsePaginated<WeatherStationView>(pageNumber, pageSize);
        var weatherStations = await _weatherStationService.GetAsync(paginated.PageNumber, paginated.PageSize);
        paginated.AddData(weatherStations, Request);

        return paginated.Data.Count() < 1 ? NotFound("Empty page") : paginated.TotalItems < 1 ? NotFound("Weather Stations not found") : Ok(paginated);
    }

    [HttpGet("{id:int}", Name = "GetWeatherStationById")]
    public async Task<ActionResult<WeatherStationView>> GetByIdAsync(int? id)
    {
        var weatherStationDto = await _weatherStationService.GetByIdAsync(id);
        var weatherStation = _mapper.Map<WeatherStation>(weatherStationDto);
        return weatherStation is null ? NotFound("Weather Station not found") : Ok(weatherStation);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpGet("{id:int}/Admin", Name = "GetWeatherStationById")]
    public async Task<ActionResult<WeatherStationDTO>> GetByIdAdminAsync(int id)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            if (await _weatherStationService.IsAdminManagerMainteiner(id, userEmail))
            {
                var weatherStationDto = await _weatherStationService.GetByIdAsync(id);
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
            weatherStationDto = await _weatherStationService.CreateAsync(weatherStationDto);

            return new CreatedAtRouteResult("GetWeatherStationById", new { id = weatherStationDto.Id }, weatherStationDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateAsync(int id, [FromBody] WeatherStationDTO weatherStationDto)
    {
        if (id != weatherStationDto.Id) return BadRequest("Id is different from WeatherStation.Id");

        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            if (await _weatherStationService.IsAdminManagerMainteiner(id, userEmail))
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
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> RemoveAsync(int id)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            if (await _weatherStationService.IsAdminManagerMainteiner(id, userEmail))
            {
                return await _weatherStationService.DeleteAsync(id) ? NoContent() : BadRequest("Not deleted");
            }

            return Unauthorized("Unauthorized");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // IMPLEMENTAR: ao criar estação, fazer vínuculo com usuário que a criou [só testar se está funcionando o que fiz]

    // implementar endpoint que retorna usuários de uma estação by weatherStationId
    //  [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]

    // endpoint POST "User/Maintainer" + json com e-mail do usuário e id da estação
    // GET e-mail from JWT token e busca o usuário [e-mail] na lista de usuários da estação [id]
    // verifica se o POST foi feito por um Maintainer da estação cujo id está no JSON:
    //              -> vincula o novo usuário como maintener da estação meteorológica
    // se não:
    //              -> Return 401 Unauthorized
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpPost("{id:int}/Maintainers")]
    public async Task<ActionResult> AddMaintainer([FromBody] WeatherStationIdUserId weatherStationUser)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            if (await _weatherStationService.IsAdminManagerMainteiner(weatherStationUser.WeatherStationId, userEmail))
            {
                return await _weatherStationService.AddMaintainer(weatherStationUser) ? Ok() : BadRequest("Not added");
            }

            return Unauthorized("Unauthorized");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // remove Maintainers
    [HttpDelete("{id:int}/Maintainers/{maintainer:int}")]

    // Get Maintainers
    // [HttpGet("{id:int}/Maintainers")]

    // os mesmo 3 para favoritos, com exceçãoq ue o return favoritos vai ser por id de usuario


    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpGet("{id:int}/Authentication", Name = "GetWeatherStationAuthentication")]
    public async Task<ActionResult<WeatherStationAuthenticationDTO>> GetWeatherStationAuthentication(int id)
    {
        try
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail is null) return BadRequest("O token JWT não contém o email do usuário.");

            if (await _weatherStationService.IsAdminManagerMainteiner(id, userEmail))
            {
                // reset 
            }

            return Unauthorized("User Unauthorized");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        // se o usuário é Maintainer, ver se o usuário que está fazendo a requisição é mantenedor da estação
        // implementar  
        //através do bearer token, saber se o usuário pode ter acesso a isso
        //Quem poderá acessar: mantenedor da estação ou manager / admin da plataforma;

        // Se o email não estiver presente no token, retorna um erro


        //// find user to get id
        //var user = await _userManager.FindByEmailAsync(userEmail);
        //var roles = await _userManager.GetRolesAsync(user);
        //var role = roles.FirstOrDefault();

        //if (role == UserRoles.User)
        //{
        //    await _userManager.RemoveFromRolesAsync(user, roles);
        //    _userManager.AddToRoleAsync(user, UserRoles.Maintainer).Wait();
        //}



        var weatherStation = await _weatherStationService.GetWeatherStationAuthentication(id, userEmail);
        return weatherStation is null ? NotFound("WeatherStation not found") : Ok(weatherStation);
    }


}
