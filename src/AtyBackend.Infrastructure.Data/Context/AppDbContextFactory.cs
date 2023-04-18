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

            //var stringConnection = "Data Source=localhost;User ID=SA;Password=pw2020@mssql;Initial Catalog=pdm-dev;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            //var stringConnection = "Data Source=localhost;User ID=SA;Password=mssql1Ipw;Initial Catalog=pdm-dev;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            // aws romildo
            var stringConnection = "Data Source=3.87.196.133,1433;User ID=SA;Password=pw2020@mssql;Initial Catalog=pdm-dev;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            optionsBuilder.UseSqlServer(stringConnection);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}