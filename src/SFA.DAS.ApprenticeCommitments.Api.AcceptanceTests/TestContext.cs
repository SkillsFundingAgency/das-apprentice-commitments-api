using System;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Infrastructure;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests
{
    public class TestContext : IDisposable
    {
        private bool isDisposed;

        public TestContext()
        {
            isDisposed = false;
        }
        public ApprenticeCommitmentsApi Api { get; set; }
        public ApprenticeCommitmentsDbContext DbContext { get; set; }
        public SpecifiedTimeProvider Time { get; set; }
            = new SpecifiedTimeProvider(DateTimeOffset.UtcNow);

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