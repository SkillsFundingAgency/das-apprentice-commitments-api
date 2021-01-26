using System;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Database;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests
{
    public class TestContext : IDisposable
    {
        private bool isDisposed;

        public TestContext()
        {
            isDisposed = false;
            DatabaseConnectionString = $"Data Source={CreateAcceptanceTestData.AcceptanceTestsDatabaseName}";
        }
        public ApprenticeCommitmentsApi Api { get; set; }
        public string DatabaseConnectionString { get; set; }

        public Action<DbContext> PopulateData { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                Api?.Dispose();
            }

            isDisposed = true;
        }
    }
}