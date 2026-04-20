using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.RegistrationByIdQuery
{
    public class RegistrationByIdQueryHandler : IRequestHandler<RegistrationByIdQuery, Registration>
    {
        private readonly IRegistrationContext _registrations;

        public RegistrationByIdQueryHandler(IRegistrationContext registrations)
            => _registrations = registrations;

        public async Task<Registration> Handle(RegistrationByIdQuery  query, CancellationToken cancellationToken)
        {
            var result = await _registrations.GetById(query.RegistrationId);

            if (result == null) return null;

            return result;
        }
    }
}
