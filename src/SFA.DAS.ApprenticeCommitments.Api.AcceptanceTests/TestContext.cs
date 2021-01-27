using System;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests
{
    public class TestContext : IDisposable
    {
        private bool isDisposed;

        public TestContext()
        {
            isDisposed = false;
            DatabaseConnectionString = $"Data Source={AcceptanceTestsData.AcceptanceTestsDatabaseName}";
        }
        public ApprenticeCommitmentsApi Api { get; set; }
        public string DatabaseConnectionString { get; set; }
        public ApprenticeCommitmentsDbContext DbContext { get; set; }

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