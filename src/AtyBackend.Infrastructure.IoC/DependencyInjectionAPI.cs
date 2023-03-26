using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AtyBackend.Application.Interfaces;
using AtyBackend.Application.Mappings;
using AtyBackend.Application.Services;
using AtyBackend.Domain.Account;
using AtyBackend.Domain.Interfaces;
using AtyBackend.Infrastructure.Data.Context;
using AtyBackend.Infrastructure.Data.Identity;
using AtyBackend.Infrastructure.Data.Repositories;

namespace AtyBackend.Infrastructure.IoC;

public static class DependencyInjectionAPI
{
    public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
         options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"], b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IExemploRepository, ExemploRepository>();
        
        services.AddScoped<IExemploService, ExemploService>();
        services.AddScoped<IExemploGenericService, ExemploGenericService>();

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IAuthenticate, AuthenticateService>();

        services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

        var myhandlers = AppDomain.CurrentDomain.Load("AtyBackend.Application");
        services.AddMediatR(myhandlers);

        // add seedUsers

        return services;
    }

}