using System.ComponentModel.DataAnnotations.Schema;

namespace AtyBackend.Application.DTOs;

public abstract class EntityDTO
{
    public int Id { get; set; }
    public bool IsEnabled { get; set; }
}
