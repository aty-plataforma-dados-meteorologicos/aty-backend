using AtyBackend.Domain.Enums;

namespace AtyBackend.Domain.Entities;

public class Exemplo : Entity
{
    public string? Name { get; set; }
    public ExemploTypeEnum ExemploType { get; set; }
}