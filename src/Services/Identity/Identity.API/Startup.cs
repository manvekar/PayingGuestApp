﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityServer4.Services;
using FoodApp.Services.Identity.API.Data;
using FoodApp.Services.Identity.API.Models;
using FoodApp.Services.Identity.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using FoodApp.Services.Identity.API.Certificates;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceFabric;
using Microsoft.Extensions.HealthChecks; //Manjunath - added manually

namespace FoodApp.Services.Identity.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            RegisterAppInsights(services);

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(Configuration["ConnectionString"],
                                    sqlServerOptionsAction: sqlOptions =>
                                    {
                                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                    }));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<AppSettings>(Configuration);

            services.AddMvc();

            if (Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)  //Manjunath - TODO: to understand this code
            {
                services.AddDataProtection(opts =>
                {
                    opts.ApplicationDiscriminator = "foodapp.identity";     //Manjunath - opts.ApplicationDiscriminator = "eshop.identity";
                })
                .PersistKeysToRedis(ConnectionMultiplexer.Connect(Configuration["DPConnectionString"]), "DataProtection-Keys"); //Manjunath - note DPConnectionString
            }

            services.AddHealthChecks(checks =>
            {
                var minutes = 1;
                if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
                {
                    minutes = minutesParsed;
                }
                checks.AddSqlCheck("Identity_Db", Configuration["ConnectionString"], TimeSpan.FromMinutes(minutes));
            });

            services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
            services.AddTransient<IRedirectService, RedirectService>();

            var connectionString = Configuration["ConnectionString"];
            var connectionStringConfigStore = Configuration["ConnectionString_IdentityConfigStore"]; 
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Adds IdentityServer
            services.AddIdentityServer(x => x.IssuerUri = "null")
                .AddSigningCredential(Certificate.Get())
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionStringConfigStore,   //Manjunath : changed from connectionString to connectionStringConfigStore
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(migrationsAssembly);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                                    sqlServerOptionsAction: sqlOptions =>
                                    {
                                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                    });
                })
                .Services.AddTransient<IProfileService, ProfileService>();

            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddAzureWebAppDiagnostics();
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            app.Map("/liveness", lapp => lapp.Run(async ctx => ctx.Response.StatusCode = 200));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            app.UseStaticFiles();


            // Make work identity server redirections in Edge and lastest versions of browers. WARN: Not valid in a production environment.
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
                await next();
            });

            app.UseForwardedHeaders();
            // Adds IdentityServer
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void RegisterAppInsights(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);
            var orchestratorType = Configuration.GetValue<string>("OrchestratorType");

            if (orchestratorType?.ToUpper() == "K8S")
            {
                // Enable K8s telemetry initializer
                services.EnableKubernetes();
            }
            if (orchestratorType?.ToUpper() == "SF")
            {
                // Enable SF telemetry initializer
                services.AddSingleton<ITelemetryInitializer>((serviceProvider) =>
                    new FabricTelemetryInitializer());
            }
        }
    }
}
