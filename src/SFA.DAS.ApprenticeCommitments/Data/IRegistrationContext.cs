using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data
{
    public interface IRegistrationContext : IEntityContext<Registration>
    {
        public IQueryable<Registration> IncludeApprenticeships() => Entities.IncludeApprenticeships();

        internal Task<Registration> GetById(Guid apprenticeId) => Entities.GetById(apprenticeId);

        internal Task<Registration?> Find(Guid apprenticeId) => Entities.FindById(apprenticeId);

        internal Task<Registration?> FindByCommitmentsApprenticeshipId(long commitmentsApprenticeshipId)
            => Entities.FindByCommitmentsApprenticeshipId(commitmentsApprenticeshipId);

        internal Task<List<Registration>> RegistrationsNeedingSignUpReminders(DateTime cutOffDateTime)
            => Entities.Where(r =>
                    r.FirstViewedOn == null && r.SignUpReminderSentOn == null && r.ApprenticeId == null &&
                    r.CreatedOn < cutOffDateTime)
                .ToListAsync(CancellationToken.None);

        public Task<bool> RegistrationsExist()
            => Entities.AnyAsync();
    }

    public static class RegistrationContextExtensions
    {
        public static IQueryable<Registration> IncludeApprenticeships(this IQueryable<Registration> registrations)
            => registrations.Include(x => x.Apprenticeship).ThenInclude(x => x.Revisions);

        public static async Task<Registration> GetByCommitmentsApprenticeshipId(
            this IQueryable<Registration> registrations,
            long commitmentsApprenticeshipId)
        {
            var registration = await registrations
                .FindByCommitmentsApprenticeshipId(commitmentsApprenticeshipId);

            return registration
                ?? throw new EntityNotFoundException(nameof(Registration), commitmentsApprenticeshipId.ToString());
        }

        public static async Task<Registration?> FindByCommitmentsApprenticeshipId(
            this IQueryable<Registration> registrations,
            long commitmentsApprenticeshipId)
        {
            return await registrations
                .FirstOrDefaultAsync(x =>
                    x.CommitmentsApprenticeshipId == commitmentsApprenticeshipId);
        }

        public static async Task<Registration> GetById(
            this IQueryable<Registration> registrations,
            Guid registrationId)
        {
            var registration = await registrations
                .FindById(registrationId);

            return registration
                ?? throw new DomainException(
                    $"Registration for Apprentice {registrationId} not found");
        }

        public static async Task<Registration?> FindById(this IQueryable<Registration> registrations,Guid registrationId)
        {
            return await registrations.FirstOrDefaultAsync(x => x.RegistrationId == registrationId);
        }
    }
}