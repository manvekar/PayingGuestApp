/*
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FoodApp.Services.Identity.API.Data
{
    public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=tcp:127.0.0.1,1433;Database=FoodApp.Services.IdentityDb;User Id=sa;Password=Pass@word;";
            //var connectionString = "Data Source=.\\sqlexpress;Initial Catalog=FoodApp.Services.IdentityDb;User Id=sa;Password=anvekar@123;";
            //for local db  
            //var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=msv.AuthDB;Trusted_Connection=True;MultipleActiveResultSets=true";
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }

    public class PresistantDbTemporaryDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
    {
        //TODO : 
        //      1. Connectionstring using helper object
        public PersistedGrantDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=tcp:127.0.0.1,1433;Database=FoodApp.Services.IdentityDb;User Id=sa;Password=Pass@word;";
            //var connectionString = "Data Source=.\\sqlexpress;Initial Catalog=FoodApp.Services.IdentityDb;User Id=sa;Password=anvekar@123;";
            var builder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            var migrationsAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;
            builder.UseSqlServer(connectionString, optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));

            OperationalStoreOptions op = new OperationalStoreOptions()
            {
                ConfigureDbContext = obuilder => obuilder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)),
                EnableTokenCleanup = true,
                TokenCleanupInterval = 30 // interval in seconds
            };
            return new PersistedGrantDbContext(builder.Options, op);
        }
    }

    public class ConfigurationDbTemporaryDbContextFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
    {
        //TODO : 
        //      1. Connectionstring using helper object
        public ConfigurationDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=tcp:127.0.0.1,1433;Database=FoodApp.Services.IdentityConfigDb;User Id=sa;Password=Pass@word;";
            //var connectionString = "Data Source=.\\sqlexpress;Initial Catalog=FoodApp.Services.IdentityConfigDb;User Id=sa;Password=anvekar@123;";
            //for local db  
            //var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=msv.AuthConfigDB;Trusted_Connection=True;MultipleActiveResultSets=true";  //localDB connnection string
            var builder = new DbContextOptionsBuilder<ConfigurationDbContext>();
            var migrationsAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;
            //optionsBuilder.UseSqlite("Data Source=blog.db");      //.UseSqlite For SQLlite
            builder.UseSqlServer(connectionString, optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));

            ConfigurationStoreOptions op = new ConfigurationStoreOptions()
            {
                ConfigureDbContext = obuilder => obuilder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)),
            };
            return new ConfigurationDbContext(builder.Options, op);
        }
    }
}
*/