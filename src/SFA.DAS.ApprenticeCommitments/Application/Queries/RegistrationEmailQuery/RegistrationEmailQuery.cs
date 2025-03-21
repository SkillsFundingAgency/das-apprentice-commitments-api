using MediatR;
using SFA.DAS.ApprenticeCommitments.Application.Queries.RegistrationQuery;
using System.Net.Mail;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.RegistrationEmailQuery
{
    public class RegistrationEmailQuery : IRequest<RegistrationResponse>
    {
        public required MailAddress EmailAddress { get; set; }
    }
}
