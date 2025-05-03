using MediatR;
using SFA.DAS.ApprenticeCommitments.Application.Queries.RegistrationQuery;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.RegistrationEmailQuery
{
    public class RegistrationEmailQueryHandler : IRequestHandler<RegistrationEmailQuery, RegistrationResponse?>
    {
        private readonly IRegistrationContext _registrations;

        public RegistrationEmailQueryHandler(IRegistrationContext registrations)
            => _registrations = registrations;

        public async Task<RegistrationResponse?> Handle(RegistrationEmailQuery query, CancellationToken cancellationToken)
        {
            var model = await _registrations.FindByEmail(query.EmailAddress);
            return Map(model);
        }


        private RegistrationResponse? Map(Registration? model)
        {
            if (model == null)
            {
                return null;
            }

            return new RegistrationResponse
            {
                RegistrationId = model.RegistrationId,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email.ToString(),
                FirstName = model.FirstName,
                HasViewedVerification = model.FirstViewedOn.HasValue,
                HasCompletedVerification = model.HasBeenCompleted
            };
        }
    }
}
