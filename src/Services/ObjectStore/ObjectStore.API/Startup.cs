using System;

using DrifterApps.Holefeeder.ObjectStore.API.Authorization;
using DrifterApps.Holefeeder.ObjectStore.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Behaviors;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;
using DrifterApps.Holefeeder.ObjectStore.Application.Queries;
using DrifterApps.Holefeeder.ObjectStore.Application.Validators;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure;

using FluentValidation;

using HealthChecks.UI.Client;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DrifterApps.Holefeeder.ObjectStore.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            _ = RegisterServices(services);

            _ = services.AddLogging();

            // Registers required services for health checks
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddMySql(Configuration["ObjectStoreDatabaseSettings:ConnectionString"],
                    "ObjectStoreDB-check",
                    tags: new[] {"object-store-db"});


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(
                    options => options.TokenValidationParameters =
                        new TokenValidationParameters {ValidateIssuer = false},
                    options => Configuration.Bind("AzureAdB2C", options));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo {Title = "ObjectStore.API", Version = "v2"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.MigrateDb();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "ObjectStore.API v2"));
            }

            app.UseRouting();

            ConfigureAuth(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        private IServiceCollection RegisterServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(GetStoreItemHandler).Assembly)
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

            AssemblyScanner.FindValidatorsInAssembly(typeof(CreateStoreItemCommandValidator).Assembly)
                .ForEach(item => services.AddScoped(item.InterfaceType, item.ValidatorType));

            services.AddTransient<Func<IRequestUser>>(provider =>
            {
                var httpContext = provider.GetService<IHttpContextAccessor>();
                return () => new RequestUserContext(httpContext?.HttpContext.User.GetUniqueId() ?? Guid.Empty);
            });

            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddScoped<ItemsCache>();

            services.AddObjectStoreDatabase(Configuration);

            return services;
        }
    }
}
