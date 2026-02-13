using MediatR;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.GetRegistrationsByAccountDetails
{
    public class GetRegistrationByAccountDetailsQuery : IRequest<List<Registration>>
    {
        public GetRegistrationByAccountDetailsQuery(string firstName, string lastName, DateTime dateOfBirth)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }

        public string FirstName { get; }
        public string LastName { get; }
        public DateTime DateOfBirth { get; }
    }
}
