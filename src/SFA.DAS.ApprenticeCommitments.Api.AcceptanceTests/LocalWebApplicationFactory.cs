using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Testing;
using SFA.DAS.ApprenticeCommitments.Infrastructure;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests
{
    public class LocalWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private readonly Dictionary<string, string> _config;
        private readonly Func<ITimeProvider> _timeProvider;
        private readonly Func<ConcurrentBag<PublishedEvent>> _events;

        public LocalWebApplicationFactory(Dictionary<string, string> config, Func<ITimeProvider> timeProvider, Func<ConcurrentBag<PublishedEvent>> events)
        {
            _config = config;
            _timeProvider = timeProvider;
            _events = events;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(s =>
            {
                s.AddTransient<IConnectionFactory, TestsDbConnectionFactory>();
                s.AddTransient((_) => _timeProvider());
                s.AddTransient<IMessageSession>((_) => new TestEventPublisher(_events.Invoke()));
            });

            builder.ConfigureAppConfiguration(a =>
            {
                a.Sources.Clear();
                a.AddInMemoryCollection(_config);
            });
            builder.UseEnvironment("Development");
        }
    }
}
