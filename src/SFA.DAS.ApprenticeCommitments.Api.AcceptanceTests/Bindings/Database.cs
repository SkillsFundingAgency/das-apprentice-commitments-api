using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Extensions;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Bindings
{
    [Binding]
    [Scope(Tag = "database")]
    public class Database
    {
        private readonly TestContext _context;
        private TestsDbConnectionFactory dbFactory = new TestsDbConnectionFactory();

        public Database(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario()]
        public void Initialise()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApprenticeCommitmentsDbContext>();
            var options = dbFactory
                .AddConnection(optionsBuilder)
                .EnableSensitiveDataLogging()
                .Options;
            _context.DbContext = new UntrackedApprenticeCommitmentsDbContext(options);

            dbFactory.EnsureCreated(_context.DbContext);
        }

        [AfterScenario()]
        public void Cleanup()
        {
            dbFactory.EnsureDeleted(_context.DbContext);
            _context?.DbContext?.Dispose();
        }
    }
}