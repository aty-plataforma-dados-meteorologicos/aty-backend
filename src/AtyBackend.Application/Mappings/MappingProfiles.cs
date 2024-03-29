﻿using AutoMapper;
using AtyBackend.Application.DTOs;
using AtyBackend.Domain.Entities;
using AtyBackend.Infrastructure.Data.Identity;
using AtyBackend.Application.ViewModels;

namespace AtyBackend.Application.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
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

        CreateMap<WeatherStationCreate, WeatherStationDTO>()
            //.ForMember(dest => dest.We)
            .ReverseMap();

        CreateMap<WeatherStationUserDTO, WeatherStationUser>()
            .ForPath(dest => dest.WeatherStationId, opt => opt.MapFrom(src => src.WeatherStationId))
            .ForMember(dest => dest.WeatherStation, opt => opt.Ignore())
            .ForPath(dest => dest.ApplicationUserId, opt => opt.MapFrom(src => src.ApplicationUserId))
            .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())
            .ForMember(dest => dest.IsMaintainer, opt => opt.MapFrom(src => src.IsMaintainer))
            .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => src.IsFavorite))
            .ForMember(dest => dest.IsCreator, opt => opt.MapFrom(src => src.IsCreator))
            .ForMember(dest => dest.IsDataAuthorized, opt => opt.MapFrom(src => src.IsDataAuthorized));


        CreateMap<WeatherStationUser, WeatherStationUserDTO>()
            .ForPath(dest => dest.WeatherStationId, opt => opt.MapFrom(src => src.WeatherStationId))
            .ForMember(dest => dest.WeatherStation, opt => opt.MapFrom(src => src.WeatherStation))
            .ForPath(dest => dest.ApplicationUserId, opt => opt.MapFrom(src => src.ApplicationUserId));
        // esse terei que preencher na service usando um _userRepository
        //.ForMember(dest => dest.ApplicationUser, opt => opt.Ignore());


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
            //.ForMember(dest => dest.WeatherStationUsers, opt => opt.MapFrom(src =>
            //               src.WeatherStationUsers.Select(s => new WeatherStationUser
            //               {
            //                   //WeatherStationId = s.WeatherStationId,
            //                   ApplicationUserId = s.ApplicationUserId,
            //                   IsDataAuthorized = s.IsDataAuthorized,
            //                   IsFavorite = s.IsFavorite,
            //                   IsMaintainer = IsMaintainer

            //               }).ToList()));

        CreateMap<WeatherStation, WeatherStationDTO>()
            .ForMember(dest => dest.Sensors, opt => opt.MapFrom(src =>
                src.WeatherStationSensors.Select(c => new SensorDTO
                {
                    Id = c.Sensor.Id,
                    Name = c.Sensor.Name,
                    MeasurementUnit = c.Sensor.MeasurementUnit,
                    Minimum = c.Sensor.Minimum,
                    Maximum = c.Sensor.Maximum,
                    Accuracy = c.Sensor.Accuracy,
                    MeasurementType = c.Sensor.MeasurementType,
                    IsEnabled = c.Sensor.IsEnabled
                }).ToList()));

        CreateMap<WeatherStation, WeatherStationView>()
            .ForMember(dest => dest.Sensors, opt => opt.MapFrom(src =>
                src.WeatherStationSensors.Select(c => new SensorDTO
                {
                    Id = c.Sensor.Id,
                    Name = c.Sensor.Name,
                    MeasurementUnit = c.Sensor.MeasurementUnit,
                    Minimum = c.Sensor.Minimum,
                    Maximum = c.Sensor.Maximum,
                    Accuracy = c.Sensor.Accuracy,
                    MeasurementType = c.Sensor.MeasurementType,
                    IsEnabled = c.Sensor.IsEnabled
                }).ToList()));

        CreateMap<WeatherStationView, WeatherStationAccessInfo>();
        CreateMap<WeatherStation, WeatherStationAccessInfo>();

        CreateMap<WeatherStationDTO, WeatherStationView>();

        CreateMap<Measurement, MeasurementDTO>()
            .ReverseMap();

        CreateMap<WeatherData, WeatherDataDTO>()
            .ReverseMap();

        CreateMap<WeatherDataFlux, WeatherDataFluxDTO>()
            .ReverseMap();

        CreateMap<MeasurementFlux, MeasurementFluxDTO>()
            .ReverseMap();

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
    }
}
