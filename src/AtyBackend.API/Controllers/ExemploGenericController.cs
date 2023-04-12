using AtyBackend.API.Helpers;
using AtyBackend.Application.DTOs;
using AtyBackend.Application.Interfaces;
using AtyBackend.Application.Services;
using AtyBackend.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtyBackend.API.Controllers
{
    //[EnableCors()]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExemploGenericController : ControllerBase
    {
        private readonly IExemploGenericService _exemploGenericService;
        public ExemploGenericController(IExemploGenericService exemploGenericService)
        {
            _exemploGenericService = exemploGenericService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponsePaginated<ExemploGenericDTO>>> GetExemploGenericAsync([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            //var pageNumberNotNull = pageNumber is null ? 1 : pageNumber.Value;
            //var pageSizeNotNull = pageSize is null ? 10 : pageSize.Value;
            //var url = "https://" + Request.Host.ToString() + Request.Path.ToString();
            //url = Request.IsHttps ? url : url.Replace("https", "http");

            var paginated = new ApiResponsePaginated<ExemploGenericDTO>(pageNumber, pageSize);
            var exemplos = await _exemploGenericService.GetAsync(paginated.PageNumber, paginated.PageSize);
            paginated.AddData(exemplos, Request);

            return paginated.Data is null ? NotFound("Exemplo Generic not found") : Ok(paginated);


            //var exemplos = await _exemploGenericService.GetAsync(pageSizeNotNull, pageNumberNotNull);
            //var paginado = new ApiResponsePaginated<ExemploGenericDTO>(exemplos, Request);

            //return exemplos is null ? NotFound("Generics not found") : Ok(paginado);
        }

        [HttpGet("{id:int}", Name = "GetExemploGenericById")]
        public async Task<ActionResult<ExemploGenericDTO>> GetByIdAsync(int? id)
        {
            var exemploGeneric = await _exemploGenericService.GetByIdAsync(id);
            return exemploGeneric is null ? NotFound("ExemploGeneric not found") : Ok(exemploGeneric);
        }

        [HttpPost]
        public async Task<ActionResult> AddAsync(ExemploGenericDTO exemploGenericDto)
        {
            if (exemploGenericDto == null)
            {
                return BadRequest("ExemploGeneric is null");
            }

            exemploGenericDto = await _exemploGenericService.CreateAsync(exemploGenericDto);

            return new CreatedAtRouteResult("GetExemploGenericById", new { id = exemploGenericDto.Id }, exemploGenericDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateAsync(int? id, [FromBody] ExemploGenericDTO exemploGenericDto)
        {
            
                if (id != exemploGenericDto.Id)
                {
                    return BadRequest("Id is not equal");
                }

                var result = await _exemploGenericService.UpdateAsync(exemploGenericDto);
                return (result is not null) ? Ok(result) : NotFound("Exemplo not found");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> RemoveAsync(int id) => await _exemploGenericService.DeleteAsync(id) ? NoContent() : BadRequest("Not deleted");
    }
}
