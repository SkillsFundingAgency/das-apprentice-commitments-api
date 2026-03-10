using MediatR;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.GetRegistrationByEmailQuery
{
    public class GetRegistrationByEmailQuery : IRequest<List<Registration>>
    {
        public GetRegistrationByEmailQuery(string email)
        {
            Email = email;
        }

        public string Email { get; set; }
    }
}
