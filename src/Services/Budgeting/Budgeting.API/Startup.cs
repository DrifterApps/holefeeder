using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Behaviors;
using DrifterApps.Holefeeder.Budgeting.Infrastructure;

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
using Serilog;

namespace DrifterApps.Holefeeder.Budgeting.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = RegisterServices(services);

            _ = services.AddLogging();

            // Registers required services for health checks
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddMySql(Configuration["HolefeederDatabaseSettings:ConnectionString"],
                    "HolefeederDB-check",
                    tags: new[] {"holefeeder-db"});

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(
                    options => options.TokenValidationParameters =
                        new TokenValidationParameters {ValidateIssuer = false},
                    options => Configuration.Bind("AzureAdB2C", options));

            _ = services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo {Title = "Budgeting.API", Version = "v2"});
            });

            _ = services.AddMvcCore()
                    .AddAuthorization(options =>
                    {
                        options.AddPolicy(Policies.REGISTERED_USER, policyBuilder =>
                        {
                            policyBuilder.RequireAuthenticatedUser()
                                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                        });
                    })
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.MigrateDb();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "Budgeting.API v2"));
            }

            app.UseSerilogRequestLogging();

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
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
        }

        private IServiceCollection RegisterServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(GetAccountsHandler).Assembly)
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

            services.AddTransient<Func<IRequestUser>>(provider =>
            {
                var httpContext = provider.GetService<IHttpContextAccessor>();
                return () => new RequestUserContext(httpContext?.HttpContext.User.GetUniqueId() ?? Guid.Empty);
            });

            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddScoped<ItemsCache>();

            services.AddHolefeederDatabase(Configuration);

            return services;
        }
    }
}
