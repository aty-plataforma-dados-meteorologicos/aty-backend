namespace AtyBackend.Domain.Entities
{
    public class Partner : Entity
    {
        public string Name { get; set; }
        public string? CnpjOrCPF { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        public string? Site { get; set; }
        public string? Notes { get; set; }
    }
}
