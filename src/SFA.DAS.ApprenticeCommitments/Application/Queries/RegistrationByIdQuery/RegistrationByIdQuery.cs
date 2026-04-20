using MediatR;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.RegistrationByIdQuery
{
    public class RegistrationByIdQuery : IRequest<Registration>
    {
        public Guid RegistrationId { get; set; }
    }
}
