
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
    public async Task<ActionResult<ApiResponsePaginated<WeatherStationDTO>>> GetWeatherStationAsync([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var paginated = new ApiResponsePaginated<WeatherStationDTO>(pageNumber, pageSize);
        var weatherStations = await _weatherStationService.GetAsync(paginated.PageNumber, paginated.PageSize);
        paginated.AddData(weatherStations, Request);

        return paginated.Data.Count() < 1 ? NotFound("Empty page") : paginated.TotalItems < 1 ? NotFound("Sensors not found") : Ok(paginated);
    }

    [HttpGet("{id:int}", Name = "GetWeatherStationById")]
    public async Task<ActionResult<WeatherStationDTO>> GetByIdAsync(int? id)
    {
        var weatherStation = await _weatherStationService.GetByIdAsync(id);
        return weatherStation is null ? NotFound("WeatherStation not found") : Ok(weatherStation);
    }


    [HttpPost]
    public async Task<ActionResult> AddAsync(WeatherStationCreate weatherStation)
    {
        try
        {
            if (weatherStation == null)
            {
                return BadRequest("WeatherStation is null");
            }

            var weatherStationDto = _mapper.Map<WeatherStationDTO>(weatherStation);

            // add user em weatherStationDto.[]WeatherStationUsers o IsMaintainer, pegar o id com base no toekn da request
            // get user from token jwt

            // Obtém o valor do email do usuário a partir do token JWT
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Se o email não estiver presente no token, retorna um erro
            if (userEmail is null)
            {
                return BadRequest("O token JWT não contém o email do usuário.");
            }

            // find user to get id
            var user = await _userManager.FindByEmailAsync(userEmail);
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (role == UserRoles.User)
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
                _userManager.AddToRoleAsync(user, UserRoles.Maintainer).Wait();
            }

            // add weatherStationDto.WeatherStationUsers

            weatherStationDto.WeatherStationUsers = new List<WeatherStationUserDTO>();

            var userDto = new WeatherStationUserDTO
            {
                ApplicationUserId = user.Id,
                IsMaintainer = true,
                IsDataAuthorized = true,
                IsFavorite = false
            };

            weatherStationDto.WeatherStationUsers.Add(userDto);

            //weatherStationDto = await _weatherStationService.CreateAsync(weatherStationDto);
            weatherStationDto = await _weatherStationService.CreateAsync(weatherStationDto);
            //return new CreatedAtRouteResult("GetWeatherStationById", new { id = weatherStationDto.Id }, weatherStationDto);
      

            //weatherStationDto.WeatherStationUsers.Add(new WeatherStationUserDTO
            //{
            //    ApplicationUserId = user.Id,
            //    IsMaintainer = true,
            //    IsDataAuthorized = true,
            //    IsFavorite = false
            //});


            return new CreatedAtRouteResult("GetWeatherStationById", new { id = weatherStationDto.Id }, weatherStationDto);
        } catch (Exception ex) { 
            
            return  BadRequest(ex.Message); 
        
        
        }
    }

        // add a role
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateAsync(int? id, [FromBody] WeatherStationDTO weatherStationDto)
    {
        // validar se o usuário pode editar a estação (é mantenedor da estação ou admin/manager da plataforma)
        if (id != weatherStationDto.Id)
        {
            return BadRequest("Id is different from WeatherStation.Id");
        }

        var result = await _weatherStationService.UpdateAsync(weatherStationDto);
        return (result is not null) ? Ok(result) : NotFound("WeatherStation not found");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> RemoveAsync(int id) => await _weatherStationService.DeleteAsync(id) ? NoContent() : BadRequest("Not deleted");

    // IMPLEMENTAR: ao criar estação, fazer vínuculo com usuário que a criou

    // implementar endpoint que retorna usuários de uma estação bi weather station id
    //  [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]

    // endpoint POST "User/Maintainer" + json com e-mail do usuário e id da estação
    // GET e-mail from JWT token e busca o usuário [e-mail] na lista de usuários da estação [id]
    // verifica se o POST foi feito por um Maintainer da estação cujo id está no JSON:
    //              -> vincula o novo usuário como maintener da estação meteorológica
    // se não:
    //              -> Return 401 Unauthorized
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpPost("User/Maintainer")]
    public async Task<ActionResult> AddMaintainer([FromBody] WeatherStationIdUserId weatherStationUser)
    {
        t

    }



    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager},{UserRoles.Maintainer}")]
    [HttpGet("Authentication/{id:int}", Name = "GetWeatherStationAuthentication")]
    public async Task<ActionResult<WeatherStationAuthenticationDTO>> GetWeatherStationAuthentication(int? id)
    {
        //através do bearer token, saber se o usuário pode ter acesso a isso
        //Quem poderá acessar: mantenedor da estação ou manager / admin da plataforma;
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        // Se o email não estiver presente no token, retorna um erro
        if (userEmail is null)
        {
            return BadRequest("O token JWT não contém o email do usuário.");
        }

        //// find user to get id
        //var user = await _userManager.FindByEmailAsync(userEmail);
        //var roles = await _userManager.GetRolesAsync(user);
        //var role = roles.FirstOrDefault();

        //if (role == UserRoles.User)
        //{
        //    await _userManager.RemoveFromRolesAsync(user, roles);
        //    _userManager.AddToRoleAsync(user, UserRoles.Maintainer).Wait();
        //}



        var weatherStation = await _weatherStationService.GetWeatherStationAuthentication(id);
        return weatherStation is null ? NotFound("WeatherStation not found") : Ok(weatherStation);
    }
}
