using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.GetRegistrationsByAccountDetails
{
    public class GetRegistrationByAccountDetailsQueryHandler : IRequestHandler<GetRegistrationByAccountDetailsQuery, List<Registration>>
    {
        private readonly IRegistrationContext _registrationContext;

        public GetRegistrationByAccountDetailsQueryHandler(IRegistrationContext registrationContext)
            => _registrationContext = registrationContext;

        public async Task<List<Registration>> Handle(
            GetRegistrationByAccountDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _registrationContext.FindByAccountDetails(
                request.FirstName,
                request.LastName,
                request.DateOfBirth);

            if (entity == null) return new List<Registration>();

            return entity;
        }
    }
}
