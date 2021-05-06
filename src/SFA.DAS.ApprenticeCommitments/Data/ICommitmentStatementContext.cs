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
        internal Task<Apprenticeship> FindByCommitmentsApprenticeshipId(long apprenticeshipId)
            => Entities.Include(x => x.CommitmentStatements)
                .FirstOrDefaultAsync(x => x.CommitmentStatements.Any(y =>
                    y.CommitmentsApprenticeshipId == apprenticeshipId));

        internal async Task<Apprenticeship> GetById(Guid apprenticeId, long apprenticeshipId)
            => (await FindForApprentice(apprenticeId, apprenticeshipId))
                ?? throw new DomainException(
                    $"Apprenticeship {apprenticeshipId} for {apprenticeId} not found");

        internal async Task<Apprenticeship?> FindForApprentice(Guid apprenticeId, long apprenticeshipId)
            => await Entities
                .Include(a => a.CommitmentStatements)
                .Where(
                    a => a.Id == apprenticeshipId &&
                    a.Apprentice.Id == apprenticeId)
                .FirstOrDefaultAsync();
    }

    public interface ICommitmentStatementContext : IEntityContext<CommitmentStatement>
    {
        internal async Task<List<CommitmentStatement>> FindAllForApprentice(Guid apprenticeId)
            => await Entities.Where(a => a.Apprenticeship.Apprentice.Id == apprenticeId).ToListAsync();
    }
}