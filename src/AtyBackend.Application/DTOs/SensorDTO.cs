﻿using AtyBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AtyBackend.Application.DTOs
{
    public class SensorDTO : EntityDTO
    {
        public string Name { get; set; }
        public string MeasurementUnit { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public WeatherMeasurementsEnum MeasurementType { get; set; }

        /*
         A acurácia é geralmente expressa como uma porcentagem ou como um valor absoluto em relação ao valor real ou de referência.
        Por exemplo, se uma medida digital tiver uma acurácia de ± 1%, isso significa que o resultado pode variar até 1% em relação
        ao valor real. Se o valor real for de 100, o resultado pode variar entre 99 e 101.
         */
        public double Accuracy { get; set; }

        //[JsonIgnore]
        //public List<WeatherStationSensorDTO>? WeatherStationSensors { get; set; }
    }
}
