using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using AtyBackend.Infrastructure.Data.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Application.DTOs
{
    public class WeatherStationUserDTO : EntityDTO
    {
        public int WeatherStationId { get; set; }
        public WeatherStationDTO WeatherStation { get; set; }

        public string ApplicationUserId { get; set; }

        // aqui era a interface
        public ApplicationUser? ApplicationUser { get; set; }

        public bool IsDataAuthorized { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsMaintainer { get; set; }
    }
}
