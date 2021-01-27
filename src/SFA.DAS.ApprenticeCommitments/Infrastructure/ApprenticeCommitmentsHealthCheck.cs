using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;
using System.Threading;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Data;

namespace SFA.DAS.ApprenticeCommitments.Infrastructure
{
    public class ApprenticeCommitmentsHealthCheck : IHealthCheck
    {
        private const string HealthCheckResultsDescription = "Apprentice Commitments API Health Check";
        private readonly IRegistrationRepository _registrationRepository;

        public ApprenticeCommitmentsHealthCheck(IRegistrationRepository registrationRepository)
        {
            _registrationRepository = registrationRepository;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var dbConnectionHealthy = true;
            try
            {
                await _registrationRepository.RegistrationsExist();
            }
            catch
            {
                dbConnectionHealthy = false;
            }

            return dbConnectionHealthy ? HealthCheckResult.Healthy(HealthCheckResultsDescription) : HealthCheckResult.Unhealthy(HealthCheckResultsDescription);
        }
    }
}
