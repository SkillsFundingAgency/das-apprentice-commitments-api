using FluentValidation;
using FluentValidation.Results;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SFA.DAS.ApprenticeCommitments.Exceptions
{
    // By design this class restricts construction options.
#pragma warning disable RCS1194 // Implement exception constructors.

    [Serializable]
    public class RegistrationAlreadyMatchedException : ValidationException
    {
        public RegistrationAlreadyMatchedException(Guid registrationId)
            : base("Registration is already verified", new[]
            {
                new ValidationFailure("Registration", $"Registration {registrationId} is already verified"),
            })
        { }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected RegistrationAlreadyMatchedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class RegistrationMismatchDateOfBirthException : IdentityNotVerifiedException
    {
        public RegistrationMismatchDateOfBirthException(Guid apprenticeId, Guid registrationId)
            : base($"DoB from account {apprenticeId} did not match registration {registrationId}")
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected RegistrationMismatchDateOfBirthException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class RegistrationMismatchLastNameException : IdentityNotVerifiedException
    {
        public RegistrationMismatchLastNameException(Guid apprenticeId, Guid registrationId)
            : base($"Last name from account {apprenticeId} did not match registration {registrationId}")
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected RegistrationMismatchLastNameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}