using MediatR;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class RegistrationMatched : INotification
    {
        public Apprenticeship Apprenticeship { get; }

        public RegistrationMatched(Apprenticeship apprenticeship)
        {
            Apprenticeship = apprenticeship;
        }
    }
}