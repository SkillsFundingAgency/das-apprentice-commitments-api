using MediatR;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class ApprenticeshipStopped : INotification
    {
        public Revision Revision { get;}

        public ApprenticeshipStopped(Revision revision)
            => Revision = revision;
    }
}