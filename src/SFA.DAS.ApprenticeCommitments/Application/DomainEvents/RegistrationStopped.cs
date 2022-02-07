using MediatR;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class RegistrationStopped : INotification
    {
        public Registration Registration { get; }

        public RegistrationStopped(Registration registration)
            => Registration = registration;
    }

    internal class RevisionStopped : INotification
    {
        public Revision Revision { get; }

        public RevisionStopped(Revision revision)
            => Revision = revision;
    }
}