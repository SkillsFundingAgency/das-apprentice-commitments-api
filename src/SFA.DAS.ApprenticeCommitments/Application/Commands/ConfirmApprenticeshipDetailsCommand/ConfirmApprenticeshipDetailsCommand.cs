using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmApprenticeshipDetailsCommand
{
    public class ConfirmApprenticeshipDetailsCommand : IUnitOfWorkCommand
    {
        public ConfirmApprenticeshipDetailsCommand(
            Guid apprenticeId, long apprenticeshipId,
            bool apprenticeshipDetailsCorrect)
        {
            ApprenticeId = apprenticeId;
            ApprenticeshipId = apprenticeshipId;
            ApprenticeshipDetailsCorrect = apprenticeshipDetailsCorrect;
        }

        public Guid ApprenticeId { get; }
        public long ApprenticeshipId { get; }
        public bool ApprenticeshipDetailsCorrect { get; }
    }

    public class ConfirmApprenticeshipDetailsCommandHandler
        : IRequestHandler<ConfirmApprenticeshipDetailsCommand>
    {
        private readonly IApprenticeshipContext _apprenticeships;

        public ConfirmApprenticeshipDetailsCommandHandler(IApprenticeshipContext apprenticeships)
            => _apprenticeships = apprenticeships;

        public async Task<Unit> Handle(ConfirmApprenticeshipDetailsCommand request, CancellationToken _)
        {
            var apprenticeship = await _apprenticeships.GetById(request.ApprenticeId, request.ApprenticeshipId);
            apprenticeship.ConfirmApprenticeshipDetails(request.ApprenticeshipDetailsCorrect);
            return Unit.Value;
        }
    }
}