using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.GetRegistrationByEmailQuery
{
    public class GetRegistrationByEmailQueryHandler : IRequestHandler<GetRegistrationByEmailQuery, List<Registration>>
    {
        private readonly IRegistrationContext _registrationContext;

        public GetRegistrationByEmailQueryHandler(IRegistrationContext registrationContext)
            => _registrationContext = registrationContext;

        public async Task<List<Registration>> Handle(GetRegistrationByEmailQuery request, CancellationToken cancellationToken)
        {
            var entity = await _registrationContext.FindByEmail(request.Email, cancellationToken);

            if (entity == null) return new List<Registration>();

            return entity;
        }
    }
}
