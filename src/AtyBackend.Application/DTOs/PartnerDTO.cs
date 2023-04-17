using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Application.DTOs
{
    public class PartnerDTO : EntityDTO
    {
        public string Name { get; set; }
        public string? CnpjOrCPF { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        public string? Site { get; set; }
        public string? Notes { get; set; }
    }
}
