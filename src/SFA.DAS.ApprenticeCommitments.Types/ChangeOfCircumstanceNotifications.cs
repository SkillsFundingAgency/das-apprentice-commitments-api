using System;

namespace SFA.DAS.ApprenticeCommitments.Types
{
    [Flags]
    public enum ChangeOfCircumstanceNotifications
    {
        None = 0,
        EmployerDetailsChanged = 1,
        ProviderDetailsChanged = 2,
        ApprenticeshipDetailsChanged = 4
    }
}
