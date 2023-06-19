using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Application.DTOs
{
    public class MeasurementFluxDTO
    {
        public double MeasurementValue { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
