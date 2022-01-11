using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ApprenticeCommitments.Application.Queries.RegistrationQuery;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.ApprovalsRegistrationQuery
{
    public class ApprovalsRegistrationHandler : IRequestHandler<ApprovalsRegistrationQuery, ApprovalsRegistrationResponse?>
    {
        private readonly IRegistrationContext _registrations;

        public ApprovalsRegistrationHandler(IRegistrationContext registrations)
            => _registrations = registrations;

        public async Task<ApprovalsRegistrationResponse?> Handle(ApprovalsRegistrationQuery query, CancellationToken cancellationToken)
        {
            var model = await _registrations.FindByCommitmentsApprenticeshipId(query.CommitmentsApprenticeshipId);
            return Map(model);
        }

        private ApprovalsRegistrationResponse? Map(Registration? model)
        {
            if (model == null)
            {
                return null;
            }

            return new ApprovalsRegistrationResponse
            {
                RegistrationId = model.RegistrationId,
                Email = model.Email.ToString(),
                HasApprenticeAssigned = model.ApprenticeId.HasValue
            };
        }
    }
}