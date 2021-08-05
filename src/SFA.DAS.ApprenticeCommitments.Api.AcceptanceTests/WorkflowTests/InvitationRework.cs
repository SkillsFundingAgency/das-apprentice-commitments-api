using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    public class InvitationRework : ApiFixture
    {
        // create account

        [Test]
        public async Task Cannot_find_registration_that_doesnt_exist()
        {
            var response = await PostVerifyRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Registration for Apprentice * not found\"*");
        }

        [Test]
        public async Task Cannot_find_apprentice_that_doesnt_exist()
        {
            var approval = await CreateApprenticeship();

            var response = await PostVerifyRegistrationCommand(approval.ApprenticeId, approval.ApprenticeId);

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Apprentice * not found\"*");
        }

        [Test]
        public async Task Cannot_match_incorrect_date_of_birth()
        {
            var approval = await CreateApprenticeship();

            await CreateAccount(approval.ApprenticeId, dateOfBirth: fixture.Create<DateTime>());
            var response = await PostVerifyRegistrationCommand(approval.ApprenticeId, approval.ApprenticeId);

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Sorry, your identity has not been verified, please check your details\"*");
        }

        [Test]
        public async Task Valid_match_creates_apprenticeship()
        {
            var approval = await CreateApprenticeship();
            await CreateAccount(approval.ApprenticeId, dateOfBirth: approval.DateOfBirth);

            await VerifyRegistration(approval.ApprenticeId, approval.ApprenticeId);

            var apprenticeship = GetApprenticeships(approval.ApprenticeId);
        }
    }
}