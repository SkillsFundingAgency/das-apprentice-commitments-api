using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data
{
    public interface IRevisionContext : IEntityContext<Revision>
    {

        internal async Task<Revision> GetById(long apprenticeshipId, long revisionId)
        {
            var revision = await Entities.Where(e=>e.Id == revisionId).SingleOrDefaultAsync();

            if (revision == null)
            {
                throw new DomainException($"Revision {revisionId} not found");
            }

            if (revision.ApprenticeshipId != apprenticeshipId)
            {
                throw new DomainException($"Revision {revisionId} does not belong to Apprenticeship {apprenticeshipId}");
            }

            return revision;
        }
    }
}