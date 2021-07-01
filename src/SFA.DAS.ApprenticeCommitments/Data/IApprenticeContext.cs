using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data
{
    public interface IApprenticeContext : IEntityContext<Apprentice>
    {
        internal async Task<Apprentice> GetById(Guid apprenticeId)
            => await Find(apprenticeId)
                     ?? throw new DomainException(
                        $"Apprentice {apprenticeId} not found");

        internal async Task<Apprentice?> Find(Guid apprenticeId)
            => await Entities.SingleOrDefaultAsync(a => a.Id == apprenticeId);
    }
}