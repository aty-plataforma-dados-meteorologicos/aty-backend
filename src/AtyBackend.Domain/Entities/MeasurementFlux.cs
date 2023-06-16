using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Domain.Entities
{
    public class MeasurementFlux
    {
        public double MeasurementValue { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
