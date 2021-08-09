using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;

#nullable disable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.VerifyRegistrationCommand
{
    public class VerifyRegistrationCommand2 : IUnitOfWorkCommand
    {
        public Guid RegistrationId { get; set; }
        public Guid ApprenticeId { get; set; }
    }
}