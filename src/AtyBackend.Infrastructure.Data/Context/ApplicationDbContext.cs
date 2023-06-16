using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AtyBackend.Domain.Entities;
using AtyBackend.Infrastructure.Data.Identity;
using System.Linq.Expressions;

namespace AtyBackend.Infrastructure.Data.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    public DbSet<Partner> Partners { get; set; }
    public DbSet<Sensor> Sensor { get; set; }
    public DbSet<WeatherStation> WeatherStation { get; set; }
    public DbSet<WeatherStationSensor> WeatherStationSensor { get; set; }
    public DbSet<WeatherStationUser> WeatherStationUser { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // soft delete 
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.GetProperty("IsDeleted") != null)
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var body = Expression.Equal(
                    Expression.Property(parameter, "IsDeleted"),
                    Expression.Constant(false));
                var lambda = Expression.Lambda(body, parameter);

                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Entity<WeatherStationSensor>()
           .HasKey(wsu => new { wsu.WeatherStationId, wsu.SensorId });

        // Alternative
        //builder.Entity<WeatherStationSensor>(
        //wss =>
        //{
        //    wss.HasKey(wsu => new { wsu.WeatherStationId, wsu.SensorId });
        //    wss.HasOne(wss => wss.WeatherStation)
        //        .WithMany(ws => ws.WeatherStationSensors)
        //        .HasForeignKey(w => new { w.WeatherStationId });
        //    wss.HasOne(wss => wss.Sensor);
        //});

        builder.Entity<WeatherStationUser>()
               .HasKey(wsu => new { wsu.WeatherStationId, wsu.ApplicationUserId });

        #region notes
        //builder.Entity<WeatherStationUser>()

        //builder.Entity<WeatherStationUser>()
        //    .HasKey(wsu => new { wsu.WeatherStationId, wsu.ApplicationUserId })
        //    .HasOne(wsu => wsu.ApplicationUser)
        //    .WithMany()
        //    .HasForeignKey(wsu => wsu.ApplicationUserId);


        //builder.Entity<WeatherStationUser>()
        //    .HasOne(wsu => wsu.ApplicationUser)
        //    .WithMany()
        //    .HasForeignKey(wsu => wsu.ApplicationUserId);

        //builder.Entity<WeatherStationUser>()
        //    .HasOne(wsu => wsu.WeatherStation)
        //    .WithMany(ws => ws.WeatherStationUsers)
        //    .HasForeignKey(wsu => wsu.WeatherStationId);

        // Configure a chave composta como a chave primária da entidade
        #endregion
    }
}
