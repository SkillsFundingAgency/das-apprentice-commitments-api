using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Models;

namespace SFA.DAS.ApprenticeCommitments.Data
{
    public interface IRegistrationRepository
    {
        public Task Add(RegistrationModel model);
        public Task<bool> RegistrationsExist();
    }
}
