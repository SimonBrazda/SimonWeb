using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CL3C.Services
{
    public class CL3CContextFactory : IDesignTimeDbContextFactory<CL3CContext>
    {
        public CL3CContext CreateDbContext(string[] args)
            {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        
            DbContextOptionsBuilder<CL3CContext> dbContextBuilder = new DbContextOptionsBuilder<CL3CContext>();
        
            var connectionString = configuration
                        .GetConnectionString("CL3CDbConnectionString");
        
            dbContextBuilder.UseMySql(connectionString);
        
            return new CL3CContext(dbContextBuilder.Options);
        }
    }
}