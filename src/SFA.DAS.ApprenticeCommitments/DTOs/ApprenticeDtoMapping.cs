using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public static class ApprenticeDtoMapping
    {
        [return: NotNullIfNotNull("apprenticeship")]
        public static ApprenticeDto? MapToApprenticeDto(this Apprentice? apprentice)
        {
            if (apprentice == null) return null;

            return new ApprenticeDto
            {
                Id = apprentice.Id,
                FirstName = apprentice.FirstName,
                LastName = apprentice.LastName,
                Email = apprentice.Email.ToString(),
                DateOfBirth = apprentice.DateOfBirth,
                TermsOfUseAccepted = apprentice.TermsOfUseAccepted
            };
        }
        internal static JsonPatchDocument<Apprentice> MapToApprentice(JsonPatchDocument<ApprenticeUpdateDto> updates)
        {
            var operations = updates.Operations.ConvertAll(o =>
                 new Operation<Apprentice>(o.op, o.path, o.from,
                     o.path switch { "/Email" => new MailAddress(o.value.ToString()), _ => o.value }));
            return new JsonPatchDocument<Apprentice>(operations, new DefaultContractResolver());
        }
    }
}