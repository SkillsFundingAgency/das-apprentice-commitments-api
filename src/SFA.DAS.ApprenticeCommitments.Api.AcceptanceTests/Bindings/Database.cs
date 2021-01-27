using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Bindings
{
    [Binding]
    [Scope(Tag = "database")]
    public class Database
    {
        private readonly TestContext _context;

        public Database(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario()]
        public void Initialise()
        {
            AcceptanceTestsData.DropDatabase(_context.DatabaseConnectionString);
            AcceptanceTestsData.CreateTables(_context.DatabaseConnectionString);

            var optionsBuilder = new DbContextOptionsBuilder<ApprenticeCommitmentsDbContext>().UseSqlite(_context.DatabaseConnectionString);
            _context.DbContext = new ApprenticeCommitmentsDbContext(optionsBuilder.Options);
        }

        [AfterScenario()]
        public void Cleanup()
        {
            _context?.DbContext?.Dispose();
            AcceptanceTestsData.DropDatabase(_context.DatabaseConnectionString);
        }
    }
}