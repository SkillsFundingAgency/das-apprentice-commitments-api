using MediatR;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class ApprenticeEmailAddressChanged : INotification
    {
        public Apprentice Apprentice { get; }

        public ApprenticeEmailAddressChanged(Apprentice apprentice)
            => Apprentice = apprentice;
    }
}