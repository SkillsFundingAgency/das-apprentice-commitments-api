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
    public interface IApprenticeshipContext : IEntityContext<Apprenticeship>
    {
        internal async Task<List<Apprenticeship>> FindByApprenticeId(Guid apprenticeId)
            => await Entities.Where(a => a.Apprentice.Id == apprenticeId).ToListAsync();

        internal async Task<Apprenticeship> GetById(Guid apprenticeId, long apprenticeshipId)
            => (await Find(apprenticeId, apprenticeshipId))
                ?? throw new DomainException(
                    $"Apprenticeship {apprenticeshipId} for {apprenticeId} not found");

        internal async Task<Apprenticeship?> Find(Guid apprenticeId, long apprenticeshipId)
            => await Entities
                .Where(
                    a => a.ApprenticeshipId == apprenticeshipId &&
                    a.Apprentice.Id == apprenticeId)
                .OrderByDescending(x => x.ApprovedOn)
                .FirstOrDefaultAsync();
    }
}