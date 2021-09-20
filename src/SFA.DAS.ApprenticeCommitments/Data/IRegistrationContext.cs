using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data
{
    public interface IRegistrationContext : IEntityContext<Registration>
    {
        internal async Task<Registration> GetById(Guid apprenticeId)
            => (await Find(apprenticeId))
                ?? throw new DomainException($"Registration for Apprentice {apprenticeId} not found");

        internal async Task<Registration?> Find(Guid apprenticeId)
            => await Entities.FirstOrDefaultAsync(x => x.RegistrationId == apprenticeId);

        internal async Task<Registration?> FindByCommitmentsApprenticeshipId(long commitmentsApprenticeshipId)
            => await Entities.FirstOrDefaultAsync(x => x.CommitmentsApprenticeshipId == commitmentsApprenticeshipId);

        internal Task<List<Registration>> RegistrationsNeedingSignUpReminders(DateTime cutOffDateTime)
        {
            return Entities.Include(e => e.Apprenticeship)
                .Where(r =>
                    (r.Apprenticeship == null || r.Apprenticeship.ConfirmedOn == null) &&
                    r.SignUpReminderSentOn == null && r.CreatedOn < cutOffDateTime)
                .ToListAsync();
        }

        public Task<bool> RegistrationsExist()
            => Entities.AnyAsync();
    }
}