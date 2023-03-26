using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AtyBackend.Domain.Entities;
using AtyBackend.Infrastructure.Data.Identity;

namespace AtyBackend.Infrastructure.Data.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    public DbSet<Exemplo> Exemplo { get; set; }
    public DbSet<ExemploGeneric> ExemploGeneric { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // add aqui o soft delete 

        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Entity<WeatherStationUser>()
           .HasKey(ii => new { ii.WeatherStationId, ii.ApplicationUserId });
    }
}
