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



        builder.Entity<WeatherStationSensor>()
           .HasKey(wsu => new { wsu.WeatherStationId, wsu.SensorId });

        //builder.Entity<WeatherStationUser>()

        //builder.Entity<WeatherStationUser>()
        //    .HasKey(wsu => new { wsu.WeatherStationId, wsu.ApplicationUserId })
        //    .HasOne(wsu => wsu.ApplicationUser)
        //    .WithMany()
        //    .HasForeignKey(wsu => wsu.ApplicationUserId);

        builder.Entity<WeatherStationUser>()
               .HasKey(wsu => new { wsu.WeatherStationId, wsu.ApplicationUserId });
        
        //builder.Entity<WeatherStationUser>()
        //    .HasOne(wsu => wsu.ApplicationUser)
        //    .WithMany()
        //    .HasForeignKey(wsu => wsu.ApplicationUserId);

        //builder.Entity<WeatherStationUser>()
        //    .HasOne(wsu => wsu.WeatherStation)
        //    .WithMany(ws => ws.WeatherStationUsers)
        //    .HasForeignKey(wsu => wsu.WeatherStationId);

        // Configure a chave composta como a chave primária da entidade

    }
}
