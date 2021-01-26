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

        private readonly TestContext _context;

        public Api(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario()]
        public void Initialise()
        {
            if (Client == null)
            {
                var config = new Dictionary<string, string>
                {
                    { "EnvironmentName", "ACCEPTANCE_TESTS" },
                    { "ApplicationSettings:DbConnectionString", _context.DatabaseConnectionString }
                };

                Factory = new LocalWebApplicationFactory<Startup>(config, _context);
                Client = new ApprenticeCommitmentsApi(Factory.CreateClient());
            }

            _context.Api = Client;
        }
    }
}
