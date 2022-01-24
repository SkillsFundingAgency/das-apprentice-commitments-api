using MediatR;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class ApprenticeshipStopped : INotification
    {
        public Registration Registration { get; }

        public ApprenticeshipStopped(Registration registration)
            => Registration = registration;
    }
}