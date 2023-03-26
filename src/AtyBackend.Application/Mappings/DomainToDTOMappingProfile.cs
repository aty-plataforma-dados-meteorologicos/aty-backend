using AutoMapper;
using AtyBackend.Application.DTOs;
using AtyBackend.Domain.Entities;

namespace AtyBackend.Application.Mappings;

public class DomainToDTOMappingProfile : Profile
{
    public DomainToDTOMappingProfile()
    {
        CreateMap<EntityDTO, Entity>()
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<ExemploDTO, Exemplo>()
            .ReverseMap();

        CreateMap<ExemploGenericDTO, ExemploGeneric>()
            .ReverseMap();
    }
}
