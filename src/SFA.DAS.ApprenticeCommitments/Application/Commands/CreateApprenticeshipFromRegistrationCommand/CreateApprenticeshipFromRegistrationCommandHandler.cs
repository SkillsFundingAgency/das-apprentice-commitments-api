using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Configuration;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.FuzzyMatching;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.CreateApprenticeshipFromRegistrationCommand
{
    public class CreateApprenticeshipFromRegistrationCommandHandler : IRequestHandler<CreateApprenticeshipFromRegistrationCommand, IResult>
    {
        private readonly IRegistrationContext _registrations;
        private readonly ApplicationSettings _applicationSettings;
        private readonly ILogger<CreateApprenticeshipFromRegistrationCommandHandler> _logger;

        public CreateApprenticeshipFromRegistrationCommandHandler(
            IRegistrationContext registrations,
            ApplicationSettings applicationSettings,
            ILogger<CreateApprenticeshipFromRegistrationCommandHandler> logger)
        {
            _registrations = registrations;
            _applicationSettings = applicationSettings;
            _logger = logger;
        }

        public async Task<IResult> Handle(CreateApprenticeshipFromRegistrationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Create apprenticeship for apprentice {request.ApprenticeId} from registration {request.RegistrationId}");

            var registration = await _registrations.GetById(request.RegistrationId);

            var matcher = new FuzzyMatcher(_applicationSettings.FuzzyMatchingSimilarityThreshold); 

            return registration.AssociateWithApprentice(request.ApprenticeId, request.LastName, request.DateOfBirth, matcher);
        }
    }
}