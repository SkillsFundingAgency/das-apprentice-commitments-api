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
    public interface ICommitmentStatementContext : IEntityContext<CommitmentStatement>
    {
        internal async Task<List<CommitmentStatement>> FindByApprenticeId(Guid apprenticeId)
            => await Entities.Where(a => a.Apprenticeship.Apprentice.Id == apprenticeId).ToListAsync();

        internal async Task<CommitmentStatement> GetById(Guid apprenticeId, long apprenticeshipId)
            => (await Find(apprenticeId, apprenticeshipId))
                ?? throw new DomainException(
                    $"Apprenticeship {apprenticeshipId} for {apprenticeId} not found");

        internal async Task<CommitmentStatement?> Find(Guid apprenticeId, long apprenticeshipId)
            => await Entities
                .Where(
                    a => a.ApprenticeshipId == apprenticeshipId &&
                    a.Apprenticeship.Apprentice.Id == apprenticeId)
                .OrderByDescending(x => x.ApprovedOn)
                .FirstOrDefaultAsync();
    }
}