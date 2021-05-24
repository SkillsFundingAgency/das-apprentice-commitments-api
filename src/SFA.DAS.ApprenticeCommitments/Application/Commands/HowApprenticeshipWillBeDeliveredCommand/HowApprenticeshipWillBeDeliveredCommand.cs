using MediatR;
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
            Guid apprenticeId, long apprenticeshipId,
            bool howApprenticeshipDeliveredCorrect)
        {
            ApprenticeId = apprenticeId;
            ApprenticeshipId = apprenticeshipId;
            HowApprenticeshipDeliveredCorrect = howApprenticeshipDeliveredCorrect;
        }

        public Guid ApprenticeId { get; }
        public long ApprenticeshipId { get; }
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
            var apprenticeship = await _apprenticeships.GetById(request.ApprenticeId, request.ApprenticeshipId);
            apprenticeship.ConfirmHowApprenticeshipWillBeDelivered(request.HowApprenticeshipDeliveredCorrect);
            return Unit.Value;
        }
    }
}
