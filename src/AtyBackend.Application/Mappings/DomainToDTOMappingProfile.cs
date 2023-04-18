using AutoMapper;
using AtyBackend.Application.DTOs;
using AtyBackend.Domain.Entities;

namespace AtyBackend.Application.Mappings;

public class DomainToDTOMappingProfile : Profile
{
    public DomainToDTOMappingProfile()
    {
        CreateMap<EntityDTO, Entity>()
            //.ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<SensorDTO, Sensor>()
            //.ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ReverseMap();

        // as dependencias da estação
        CreateMap<PartnerDTO, Partner>()
            .ReverseMap();

        // a estação
        CreateMap<WeatherStationDTO, WeatherStation>()
            .ForMember(dest => dest.Partners, opt => opt.MapFrom(src =>
                src.Sensors.Select(c => new WeatherStationSensor
                {
                    Sensor = new Sensor
                    {
                        Name = c.Name,
                        MeasurementUnit = c.MeasurementUnit,
                        Minimum = c.Minimum,
                        Maximum = c.Maximum,
                        Accuracy = c.Accuracy
                    }
                }).ToList()));

        CreateMap<ExemploDTO, Exemplo>()
            .ReverseMap();

        CreateMap<ExemploGenericDTO, ExemploGeneric>()
            .ReverseMap();
    }
}
