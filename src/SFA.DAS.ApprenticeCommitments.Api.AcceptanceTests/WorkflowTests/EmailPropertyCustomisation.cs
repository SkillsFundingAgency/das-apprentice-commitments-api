using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateAccountCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateApprenticeshipFromRegistrationCommand;
using System;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{

    internal class EmailPropertyCustomisation : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (!(request is PropertyInfo pip)) return new NoSpecimen();
            if (pip.Name != "Email") return new NoSpecimen();
            return context.Create<MailAddress>().ToString();
        }
    }
}