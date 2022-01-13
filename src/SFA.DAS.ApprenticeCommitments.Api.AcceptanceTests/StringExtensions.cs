using System;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests
{
    public static class StringExtensions
    {
        public static Guid? ToGuid(this string value)
        {
            Guid? id = null;
            if (!string.IsNullOrWhiteSpace(value))
            {
                id = Guid.Parse(value);
            }

            return id;
        }
    }
}
