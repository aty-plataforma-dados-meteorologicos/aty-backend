using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Application.ViewModels
{
    public class WeatherStationIdUserId
    {
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }
        [Required]
        public int WeatherStationId { get; set; }
    }
}
