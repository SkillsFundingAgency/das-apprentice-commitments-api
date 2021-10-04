using System;

namespace SFA.DAS.ApprenticeCommitments.Types
{
    [Flags]
    public enum ChangeOfCircumstanceNotification
    {
        None = 0,
        EmployerDetailsChanged = 2,
        ProviderDetailsChanged = 4,
        ApprenticeshipDetailsChanged = 8
    }
}
