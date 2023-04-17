namespace AtyBackend.Domain.Entities;

public abstract class Entity
{
    public int Id { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDeleted { get; set; }
}
    