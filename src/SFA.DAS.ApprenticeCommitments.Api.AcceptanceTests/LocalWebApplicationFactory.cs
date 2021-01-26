using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Database;
using SFA.DAS.ApprenticeCommitments.Configuration;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Extensions;
using SFA.DAS.ApprenticeCommitments.Infrastructure;
using SFA.DAS.UnitOfWork.Managers;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests
{
    public class LocalWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private readonly Dictionary<string, string> _config;
        private readonly TestContext _context;

        public LocalWebApplicationFactory(Dictionary<string, string> config, TestContext context)
        {
            _config = config;
            _context = context;
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(s =>
            {
                s.AddEntityFrameworkSqlite();
                s.AddTransient<IUnitOfWorkManager, FakeUnitOfWorkManager>();
                s.AddTransient<IConnectionFactory, SqLiteConnectionFactory>();

                InitialiseTestDatabase(s);
            });

            builder.ConfigureAppConfiguration(a =>
            {
                a.Sources.Clear();
                a.AddInMemoryCollection(_config);
            });
            builder.UseEnvironment("LOCAL");
        }

        private void InitialiseTestDatabase(IServiceCollection serviceCollection)
        {
            var sp = serviceCollection.BuildServiceProvider();
            var connectionFactory = sp.GetService<IConnectionFactory>();

            var settings = sp.GetService<IOptions<ApplicationSettings>>().Value;

            CreateAcceptanceTestData.DropDatabase(settings.DbConnectionString);
            CreateAcceptanceTestData.CreateTables(settings.DbConnectionString);

            var optionsBuilder = new DbContextOptionsBuilder<ApprenticeCommitmentsDbContext>().UseDataStorage(connectionFactory, settings.DbConnectionString);
            using var dbContext = new ApprenticeCommitmentsDbContext(optionsBuilder.Options);

            if (_context.PopulateData != null)
            {
                _context.PopulateData.Invoke(dbContext);
            }
        }
    }
}
