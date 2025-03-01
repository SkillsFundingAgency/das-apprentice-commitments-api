using FluentValidation;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.ApprenticeCommitments.Api.AppStart;
using SFA.DAS.ApprenticeCommitments.Api.Authentication;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Configuration;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using SFA.DAS.ApprenticeCommitments.Extensions;
using SFA.DAS.ApprenticeCommitments.Infrastructure;
using SFA.DAS.Configuration.AzureTableStorage;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace SFA.DAS.ApprenticeCommitments.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            if (!Configuration.IsAcceptanceOrDev())
            {
                config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                });
            }

            if (!Configuration.IsAcceptanceTest())
            {
                config.AddJsonFile($"appsettings.Development.json", optional: true);
            }

            Configuration = config.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddTelemetryRegistration(Configuration)
                .AddApplicationInsightsTelemetry();

            services.AddSwaggerGen();

            services.Configure<AzureActiveDirectoryConfiguration>(Configuration.GetSection("AzureAd"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<AzureActiveDirectoryConfiguration>>().Value);

            if (!Configuration.IsLocalAcceptanceOrDev())
            {
                var azureAdConfiguration = Configuration
                    .GetSection("AzureAd")
                    .Get<AzureActiveDirectoryConfiguration>();

                services.AddApiAuthentication(azureAdConfiguration);
            }

            services.AddOptions();
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));
            services.AddSingleton(s => s.GetRequiredService<IOptions<ApplicationSettings>>().Value);

            services.AddEntityFrameworkForApprenticeCommitments(Configuration);

           services.AddServicesForApprenticeCommitments();
            
            services.AddHealthChecks()
                .AddCheck<ApprenticeCommitmentsHealthCheck>(nameof(ApprenticeCommitmentsHealthCheck));

            services
                .AddControllers(o =>
                {
                    if (!Configuration.IsLocalAcceptanceOrDev())
                    {
                        o.Filters.Add(new AuthorizeFilter(PolicyNames.Default));
                    }
                });

            services.AddControllersWithViews()
                .AddNewtonsoftJson();

            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<CreateRegistrationCommandValidator>();

            services.AddProblemDetails(ConfigureProblemDetails);

            var overdueDays = Configuration.GetValue<int?>("DaysUntilCommitmentStatementOverdue");
            if (overdueDays > 0) Revision.DaysBeforeOverdue = overdueDays.Value;
        }

        private void ConfigureProblemDetails(Hellang.Middleware.ProblemDetails.ProblemDetailsOptions o)
        {
            o.ValidationProblemStatusCode = StatusCodes.Status400BadRequest;
            o.Map<ValidationException>(ex => ex.ToProblemDetails());
            o.Map<DomainException>(ex => ex.ToProblemDetails());
            o.Map<EntityNotFoundException>(ex => ex.ToProblemDetails());
            o.MapToStatusCode<DBConcurrencyException>(StatusCodes.Status409Conflict);
            o.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseProblemDetails();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Apprentice Commitments API");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = (c, r) => c.Response.WriteJsonAsync(new
                {
                    r.Status,
                    r.TotalDuration,
                    Results = r.Entries.ToDictionary(
                        e => e.Key,
                        e => new
                        {
                            e.Value.Status,
                            e.Value.Duration,
                            e.Value.Description,
                            e.Value.Data
                        })
                })
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/ping");
            });
        }

        public void ConfigureContainer(UpdateableServiceProvider serviceProvider)
        {
            if (!Configuration.IsAcceptanceTest())
                serviceProvider.StartNServiceBus(Configuration).GetAwaiter().GetResult();
        }
    }
}