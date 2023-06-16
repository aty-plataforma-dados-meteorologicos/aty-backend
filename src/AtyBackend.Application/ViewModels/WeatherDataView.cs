using AtyBackend.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Application.ViewModels
{
    public class WeatherDataView
    {

        public List<SensorDTO> Sensors { get; set; }
        public List<WeatherDataDTO> Data { get; set; }
    }
}
