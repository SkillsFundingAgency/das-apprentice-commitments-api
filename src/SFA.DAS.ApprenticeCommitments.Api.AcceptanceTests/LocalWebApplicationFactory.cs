using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeCommitments.Infrastructure;
using SFA.DAS.UnitOfWork.Managers;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests
{
    public class LocalWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private readonly Dictionary<string, string> _config;

        public LocalWebApplicationFactory(Dictionary<string, string> config)
        {
            _config = config;
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(s =>
            {
                s.AddEntityFrameworkSqlite();
                s.AddTransient<IUnitOfWorkManager, FakeUnitOfWorkManager>();
                s.AddTransient<IConnectionFactory, SqLiteConnectionFactory>();
            });

            builder.ConfigureAppConfiguration(a =>
            {
                a.Sources.Clear();
                a.AddInMemoryCollection(_config);
            });
            builder.UseEnvironment("LOCAL");
        }
    }
}
