using AutoMapper;
using AtyBackend.Application.DTOs;
using AtyBackend.Domain.Entities;
using AtyBackend.Infrastructure.Data.Identity;
using AtyBackend.Application.ViewModels;

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

        CreateMap<WeatherStationCreate, WeatherStationUserDTO>()
            .ReverseMap();

        CreateMap<WeatherStationUserDTO, WeatherStationUser>()
            .ForPath(dest => dest.WeatherStationId, opt => opt.MapFrom(src => src.WeatherStation.Id))
            .ForMember(dest => dest.WeatherStation, opt => opt.Ignore())
            .ForPath(dest => dest.ApplicationUserId, opt => opt.MapFrom(src => src.ApplicationUser.Id))
            .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore());

        CreateMap<WeatherStationUser, WeatherStationUserDTO>()
            .ForPath(dest => dest.WeatherStationId, opt => opt.MapFrom(src => src.WeatherStation.Id))
            .ForMember(dest => dest.WeatherStation, opt => opt.MapFrom(src => src.WeatherStation))
            .ForPath(dest => dest.ApplicationUserId, opt => opt.MapFrom(src => src.ApplicationUserId))
            // esse terei que preencher na service usando um _userRepository
            .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore());


        // a estação
        CreateMap<WeatherStationDTO, WeatherStation>()
            .ForMember(dest => dest.WeatherStationSensors, opt => opt.MapFrom(src =>
                src.Sensors.Select(s => new WeatherStationSensor
                {
                    SensorId = s.Id
                    //Sensor = {
                    //    Name = c.Name,
                    //    MeasurementUnit = c.MeasurementUnit,
                    //    Minimum = c.Minimum,
                    //    Maximum = c.Maximum,
                    //    Accuracy = c.Accuracy
                    //}
                }).ToList()));

        CreateMap<WeatherStation, WeatherStationDTO>()
            .ForMember(dest => dest.Sensors, opt => opt.MapFrom(src =>
                src.WeatherStationSensors.Select(c => new SensorDTO
                {
                    Id = c.SensorId,
                    Name = c.Sensor.Name,
                    MeasurementUnit = c.Sensor.MeasurementUnit,
                    Minimum = c.Sensor.Minimum,
                    Maximum = c.Sensor.Maximum,
                    Accuracy = c.Sensor.Accuracy
                }).ToList()));

                //{
                //    ContainerId = c.Container.ContainerId,
                //    Type = c.Container.Type,
                //    Volume = c.Container.Volume
                //    //ImportId = src.ImportId,
                //    //ProductId = src.ProductId,
                //    //ContainerId = c.ContainerId,
                //    ////BatchId = c.BatchId,
                //    //Container = new ContainerDTO
                //    //{
                //    //    ContainerId = c.ContainerId,
                //    //    Type = c.Type,
                //    //    Volume = c.Volume
                //    //}
                //}).ToList()));



        CreateMap<ExemploDTO, Exemplo>()
            .ReverseMap();

        CreateMap<ExemploGenericDTO, ExemploGeneric>()
            .ReverseMap();
    }
}
