using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
//using CleanArchTemplate.Infrastructure.Data.Context;
using AtyBackend.Infrastructure.Data.Context;

namespace CleanArchTemplate.Infra.Data.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        
        //public AppDbContextFactory()
        //{
        //}
        ApplicationDbContext IDesignTimeDbContextFactory<ApplicationDbContext>.CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // servidor staging
            var stringConnection = "Data Source=localhost;User ID=SA;Password=root@123;Initial Catalog=pdm-dev;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            // docker user marg
            //var stringConnection = "Data Source=localhost;User ID=SA;Password=pw2020@mssql;Initial Catalog=pdm-dev;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            // docker user rom
            //var stringConnection = "Data Source=localhost;User ID=SA;Password=mssql1Ipw;Initial Catalog=pdm-dev;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            optionsBuilder.UseSqlServer(stringConnection);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
