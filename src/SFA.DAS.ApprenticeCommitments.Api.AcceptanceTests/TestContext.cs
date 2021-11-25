using NServiceBus.Testing;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Infrastructure;
using System;
using System.Collections.Concurrent;

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
            = new SpecifiedTimeProvider(DateTime.UtcNow);

        public ConcurrentBag<PublishedEvent> PublishedNServiceBusEvents { get; set; } =
            new ConcurrentBag<PublishedEvent>(); 

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