using NServiceBus.Testing;
using SFA.DAS.ApprenticeCommitments.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Bindings
{
    [Binding]
    [Scope(Tag = "api")]
    public class Api
    {
        public static ApprenticeCommitmentsApi Client { get; set; }
        public static LocalWebApplicationFactory<Startup> Factory { get; set; }
        private static readonly Func<SpecifiedTimeProvider> _time = () => _timeProvider;
        private static readonly Func<ConcurrentBag<PublishedEvent>> _events = () => _eventsProvider;
        private static SpecifiedTimeProvider _timeProvider;
        private static ConcurrentBag<PublishedEvent> _eventsProvider;

        private readonly TestContext _context;

        public Api(TestContext context)
        {
            _context = context;
            _timeProvider = context.Time;
            _eventsProvider = context.PublishedNServiceBusEvents;
        }

        [BeforeScenario()]
        public void Initialise()
        {
            if (Client == null)
            {
                Factory = CreateApiFactory();
                Client = new ApprenticeCommitmentsApi(Factory.CreateClient());
            }
            _context.Api = Client;
        }

        public static LocalWebApplicationFactory<Startup> CreateApiFactory()
        {
            var config = new Dictionary<string, string>
                {
                    { "EnvironmentName", "ACCEPTANCE_TESTS" },
                    { "ApplicationSettings:DbConnectionString", TestsDbConnectionFactory.ConnectionString },
                    { "ApplicationSettings:FuzzyMatchingSimilarityThreshold", "60" }
                };

            return new LocalWebApplicationFactory<Startup>(config, _time, _events);
        }

        [AfterFeature()]
        public static void CleanUpFeature()
        {
            Client?.Dispose();
            Client = null;
            Factory?.Dispose();
            _timeProvider = null;
            _eventsProvider = null;
        }
    }
}