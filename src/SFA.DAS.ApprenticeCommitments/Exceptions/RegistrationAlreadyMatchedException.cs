using FluentValidation;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SFA.DAS.ApprenticeCommitments.Exceptions
{
    [Serializable]
    public class RegistrationAlreadyMatchedException : ValidationException
    {
        public RegistrationAlreadyMatchedException(Guid registrationId)
            : base($"Registration {registrationId} is already verified")
        { }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected RegistrationAlreadyMatchedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}