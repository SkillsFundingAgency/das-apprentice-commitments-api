using System;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests
{
    public static class StringExtensions
    {
        public static Guid? ToGuid(this string value)
        {
            return Guid.TryParse(value, out var guid) ? guid : default;
        }
    }
}
