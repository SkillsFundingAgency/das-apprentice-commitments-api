﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using SFA.DAS.ApprenticeCommitments.Models;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.VerifyRegistrationCommand
{
    public class VerifyRegistrationCommandHandler : IRequestHandler<VerifyRegistrationCommand>
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly ApprenticeRepository _apprenticeRepository;
        private readonly ApprenticeshipRepository _apprenticeshipRepository;

        public VerifyRegistrationCommandHandler(IRegistrationRepository registrationRepository, ApprenticeRepository apprenticeRepository, ApprenticeshipRepository apprenticeshipRepository)
        {
            _registrationRepository = registrationRepository;
            _apprenticeRepository = apprenticeRepository;
            _apprenticeshipRepository = apprenticeshipRepository;
        }
        public async Task<Unit> Handle(VerifyRegistrationCommand command, CancellationToken cancellationToken)
        {
            
            var registration = await _registrationRepository.Get(command.RegistrationId);

            if (registration.HasBeenCompleted)
            {
                throw new DomainException("Already verified");
            }

            // Verify Email matches incoming email
            if (!command.Email.Equals(registration.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new DomainException("Email from Verifying user doesn't match registered user");
            }

            var apprentice = await AddApprentice(command);
            await AddApprenticeship(apprentice.Id.Value, registration);

            await _registrationRepository.CompleteRegistration(registration.Id, apprentice.Id.Value, command.UserId);

            return Unit.Value;
        }

        private async Task AddApprenticeship(long apprenticeId, RegistrationModel registration)
        {
            await _apprenticeshipRepository.Add(new ApprenticeshipModel
            {
                ApprenticeId = apprenticeId,
                CommitmentsApprenticeshipId = registration.ApprenticeshipId
            });
        }

        private async Task<ApprenticeModel> AddApprentice(VerifyRegistrationCommand command)
        {
            var apprentice = new ApprenticeModel
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                UserId = command.UserId,
                DateOfBirth = command.DateOfBirth
            };

            return await _apprenticeRepository.Add(apprentice);
        }
    }
}
