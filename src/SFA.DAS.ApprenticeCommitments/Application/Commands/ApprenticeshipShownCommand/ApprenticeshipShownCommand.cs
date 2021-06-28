using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Infrastructure;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ApprenticeshipShownCommand
{
    public class ApprenticeshipShownCommand : IUnitOfWorkCommand
    {
        public ApprenticeshipShownCommand(Guid apprenticeId, long apprenticeshipId)
        {
            ApprenticeId = apprenticeId;
            ApprenticeshipId = apprenticeshipId;
        }

        public Guid ApprenticeId { get; }
        public long ApprenticeshipId { get; }
    }

    public class ApprenticeshipShownCommandHandler : IRequestHandler<ApprenticeshipShownCommand>
    {
        private readonly IApprenticeshipContext statements;
        private readonly ITimeProvider time;

        public ApprenticeshipShownCommandHandler(IApprenticeshipContext statements, ITimeProvider time)
        {
            this.statements = statements;
            this.time = time;
        }

        public async Task<Unit> Handle(ApprenticeshipShownCommand request, CancellationToken cancellationToken)
        {
            var app = await statements.FindForApprentice(request.ApprenticeId, request.ApprenticeshipId);
            app?.ShownToApprentice(time.Now);
            return Unit.Value;
        }
    }
}