using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.DTOs;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.ApprovalsRegistrationQuery
{
    public class ApprovalsRegistrationHandler : IRequestHandler<ApprovalsRegistrationQuery, RegistrationDto?>
    {
        private readonly IRegistrationContext _registrations;

        public ApprovalsRegistrationHandler(IRegistrationContext registrations)
            => _registrations = registrations;

        public async Task<RegistrationDto?> Handle(ApprovalsRegistrationQuery query, CancellationToken cancellationToken)
        {
            var model = await _registrations.FindByCommitmentsApprenticeshipId(query.CommitmentsApprenticeshipId);

            return model?.MapToRegistrationDto();
        }
    }
}