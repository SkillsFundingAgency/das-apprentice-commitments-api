using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Map;
using SFA.DAS.ApprenticeCommitments.Models;

namespace SFA.DAS.ApprenticeCommitments.Data
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly Lazy<ApprenticeCommitmentsDbContext> _dbContext;

        public RegistrationRepository(Lazy<ApprenticeCommitmentsDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(RegistrationModel model)
        {
            await _dbContext.Value.AddAsync(model.MapToRegistration());
        }

        public async Task<bool> RegistrationsExist()
        {
            var db = _dbContext.Value;
            return await db.Registrations.AnyAsync();
        }
    }
}
