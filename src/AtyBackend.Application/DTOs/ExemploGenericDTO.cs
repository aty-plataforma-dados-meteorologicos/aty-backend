﻿using AtyBackend.Domain.Enums;

namespace AtyBackend.Application.DTOs;

public class ExemploGenericDTO : EntityDTO
{
    public string? Name { get; set; }
    public ExemploTypeEnum ExemploType { get; set; }
}
