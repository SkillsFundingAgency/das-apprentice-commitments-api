using MediatR;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class RegistrationMatched : INotification
    {
        public Registration Registration { get; }
        public Apprentice Apprentice { get; }

        public RegistrationMatched(Registration registration, Apprentice apprentice)
        {
            Registration = registration;
            Apprentice = apprentice;
        }
    }
}