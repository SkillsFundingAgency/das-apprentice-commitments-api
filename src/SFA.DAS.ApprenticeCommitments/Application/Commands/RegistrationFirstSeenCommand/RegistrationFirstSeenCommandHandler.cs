using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.RegistrationFirstSeenCommand
{
    public class RegistrationFirstSeenCommandHandler : IRequestHandler<RegistrationFirstSeenCommand>
    {
        private readonly IRegistrationContext _registrations;

        public RegistrationFirstSeenCommandHandler(IRegistrationContext registrations)
        {
            _registrations = registrations;
        }

        async Task IRequestHandler<RegistrationFirstSeenCommand>.Handle(RegistrationFirstSeenCommand request, CancellationToken cancellationToken)
        {
            var registration = await _registrations.GetById(request.ApprenticeId);
            registration.ViewedByUser(request.SeenOn);
        }
    }
}