﻿using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Configuration;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.FuzzyMatching;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.VerifyRegistrationCommand
{
    public class VerifyRegistrationCommandHandler : IRequestHandler<VerifyRegistrationCommand2>
    {
        private readonly IRegistrationContext _registrations;
        private readonly IApprenticeContext _apprentices;
        private readonly ILogger<VerifyRegistrationCommandHandler> _logger;
        private readonly ApplicationSettings _applicationSettings;

        public VerifyRegistrationCommandHandler(
            IRegistrationContext registrations,
            IApprenticeContext apprenticeRepository,
            ILogger<VerifyRegistrationCommandHandler> logger,
            ApplicationSettings applicationSettings)
        {
            _registrations = registrations;
            _apprentices = apprenticeRepository;
            _logger = logger;
            _applicationSettings = applicationSettings;
        }

        public async Task<Unit> Handle(VerifyRegistrationCommand2 request, CancellationToken cancellationToken)
        {
            var registration = await _registrations.GetById(request.RegistrationId);
            var apprentice = await _apprentices.GetById(request.ApprenticeId);

            var matcher = new FuzzyMatcher(_applicationSettings.FuzzyMatchingSimilarityThreshold);

            registration.AssociateWithApprentice(apprentice, matcher);

            return Unit.Value;
        }
    }
}