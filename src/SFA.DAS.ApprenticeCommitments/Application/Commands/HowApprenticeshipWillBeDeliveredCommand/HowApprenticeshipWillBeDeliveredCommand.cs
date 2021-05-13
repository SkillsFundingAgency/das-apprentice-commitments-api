using MediatR;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmEmployerCommand;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.HowApprenticeshipWillBeDeliveredCommand
{
    public class HowApprenticeshipWillBeDeliveredCommand : IUnitOfWorkCommand
    {
        public HowApprenticeshipWillBeDeliveredCommand(
            (Guid apprenticeId, long apprenticeshipId, long commitmentStatementId) id,
            bool howApprenticeshipDeliveredCorrect)
        {
            Id = new ApprenticeCommitmentStatementId(id);
            HowApprenticeshipDeliveredCorrect = howApprenticeshipDeliveredCorrect;
        }

        public ApprenticeCommitmentStatementId Id { get; }
        public bool HowApprenticeshipDeliveredCorrect { get; }
    }

    public class HowApprenticeshipWillBeDeliveredCommandHandler
        : IRequestHandler<HowApprenticeshipWillBeDeliveredCommand>
    {
        private readonly IApprenticeshipContext _apprenticeships;

        public HowApprenticeshipWillBeDeliveredCommandHandler(IApprenticeshipContext apprenticeships)
            => _apprenticeships = apprenticeships;

        public async Task<Unit> Handle(HowApprenticeshipWillBeDeliveredCommand request, CancellationToken cancellationToken)
        {
            var apprenticeship = await _apprenticeships.GetById(request.Id.ApprenticeId, request.Id.ApprenticeshipId);
            apprenticeship.ConfirmHowApprenticeshipWillBeDelivered(request.Id.CommitmentStatementId, request.HowApprenticeshipDeliveredCorrect);
            return Unit.Value;
        }
    }
}
