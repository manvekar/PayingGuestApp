/******DEPRECATED************/
/*
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
//using Microsoft.EntityFrameworkCore.Design;
using System.Text;

namespace FoodApp.Services.Identity.API.Data
{
 
    public class TemporaryDbContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FoodApp.Services.IdentityDb;Trusted_Connection=True;MultipleActiveResultSets=true",
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name));
            return new ApplicationDbContext(builder.Options);
        }
    }
 
    public class TemporaryDbContextFactoryScopes : IDbContextFactory<PersistedGrantDbContext>
    {
        public PersistedGrantDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=efmigrations2017;Trusted_Connection=True;MultipleActiveResultSets=true",
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(PersistedGrantDbContext).GetTypeInfo().Assembly.GetName().Name));
            return new PersistedGrantDbContext(builder.Options, new OperationalStoreOptions());
        }
    }

    public class TemporaryDbContextFactoryOperational : IDbContextFactory<ConfigurationDbContext>
    {
        public ConfigurationDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<ConfigurationDbContext>();
            builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=efmigrations2017;Trusted_Connection=True;MultipleActiveResultSets=true",
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(EFMigrations2017Context).GetTypeInfo().Assembly.GetName().Name));

            return new ConfigurationDbContext(builder.Options, new ConfigurationStoreOptions());
        }
    }
}
*/