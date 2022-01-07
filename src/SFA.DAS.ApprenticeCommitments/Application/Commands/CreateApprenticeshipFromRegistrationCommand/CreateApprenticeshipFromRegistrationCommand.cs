using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;

#nullable disable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.CreateApprenticeshipFromRegistrationCommand
{
    public class CreateApprenticeshipFromRegistrationCommand : IUnitOfWorkCommand<IResult>
    {
        public Guid RegistrationId { get; set; }
        public Guid ApprenticeId { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}