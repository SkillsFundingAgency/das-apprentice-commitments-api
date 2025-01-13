using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using NServiceBus.Persistence;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Configuration;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Extensions;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.NServiceBus.SqlServer.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServicesForApprenticeCommitments(this IServiceCollection services)
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssemblyContaining<CreateRegistrationCommand>();
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkPipelineBehavior<,>));
            });
            services.AddFluentValidation(new[] { typeof(UnitOfWorkPipelineBehavior<,>).Assembly });

            services.AddTransient<ITimeProvider, UtcTimeProvider>();
            services.AddSingleton<IManagedIdentityTokenProvider, ManagedIdentityTokenProvider>();
            services.AddTransient<IConnectionFactory, SqlServerConnectionFactory>();
            services.AddScoped<IRegistrationContext>(s => s.GetRequiredService<ApprenticeCommitmentsDbContext>());
            services.AddScoped<IApprenticeshipContext>(s => s.GetRequiredService<ApprenticeCommitmentsDbContext>());
            services.AddScoped<IRevisionContext>(s => s.GetRequiredService<ApprenticeCommitmentsDbContext>());
            services.AddScoped<EventDispatcher>();

            return services;
        }

        public static IServiceCollection AddEntityFrameworkForApprenticeCommitments(this IServiceCollection services, IConfiguration config)
        {
            return services.AddScoped(p =>
            {
                var connectionFactory = p.GetRequiredService<IConnectionFactory>();
                var loggerFactory = p.GetRequiredService<ILoggerFactory>();

                ApprenticeCommitmentsDbContext dbContext;
                var settings = p.GetRequiredService<IOptions<ApplicationSettings>>().Value;
                var optionsBuilder = new DbContextOptionsBuilder<ApprenticeCommitmentsDbContext>()
                    .UseDataStorage(connectionFactory, settings.DbConnectionString)
                    .UseLocalSqlLogger(loggerFactory, config);
                dbContext = new ApprenticeCommitmentsDbContext(optionsBuilder.Options, p.GetRequiredService<EventDispatcher>());

                return dbContext;
            });
        }

        public static async Task<UpdateableServiceProvider> StartNServiceBus(this UpdateableServiceProvider serviceProvider, IConfiguration configuration)
        {
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.ApprenticeCommitments.Api")
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                //.UseOutbox(true)
                .UseServicesBuilder(serviceProvider)
                //.UseSqlServerPersistence(() => connectionFactory.CreateConnection(configuration["ApplicationSettings:DbConnectionString"]))
                //.UseUnitOfWork()
                ;

            if (UseLearningTransport(configuration))
            {
                endpointConfiguration.UseTransport<LearningTransport>();
            }
            else
            {
                endpointConfiguration.UseAzureServiceBusTransport(configuration["ApplicationSettings:NServiceBusConnectionString"]);
            }

            if (!string.IsNullOrEmpty(configuration["ApplicationSettings:NServiceBusLicense"]))
            {
                endpointConfiguration.License(configuration["ApplicationSettings:NServiceBusLicense"]);
            }

            var endpoint = await Endpoint.Start(endpointConfiguration);

            serviceProvider.AddSingleton(p => endpoint)
                .AddSingleton<IMessageSession>(p => p.GetRequiredService<IEndpointInstance>())
                .AddHostedService<NServiceBusHostedService>();

            return serviceProvider;
        }

        private static bool UseLearningTransport(IConfiguration configuration) =>
            string.IsNullOrEmpty(configuration["ApplicationSettings:NServiceBusConnectionString"]) ||
            configuration["ApplicationSettings:NServiceBusConnectionString"].Equals("UseLearningEndpoint=true",
                StringComparison.CurrentCultureIgnoreCase);
    }
}