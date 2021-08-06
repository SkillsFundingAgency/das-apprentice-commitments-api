using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    public class InvitationRework_RegisterApproval : ApiFixture
    {
        [Test]
        public async Task Validates_command()
        {
            var create = fixture.Build<CreateRegistrationCommand>()
                .Without(p => p.CommitmentsApprenticeshipId).
                Create();
            var response = await PostCreateRegistrationCommand(create);
            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*CommitmentsApprenticeshipId*");
        }
    }
    public class InvitationRework_MatchApprenticeshipToApproval : ApiFixture
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
            var approval = await CreateRegistration();

            var response = await PostVerifyRegistrationCommand(approval.ApprenticeId, approval.ApprenticeId);

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Apprentice * not found\"*");
        }

        [Test]
        public async Task Cannot_match_incorrect_email()
        {
            var approval = await CreateRegistration();

            await CreateAccount(approval, email: fixture.Create<MailAddress>());
            var response = await PostVerifyRegistrationCommand(approval.ApprenticeId, approval.ApprenticeId);

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Sorry, your identity has not been verified, please check your details\"*");
        }

        [Test]
        public async Task Cannot_match_incorrect_date_of_birth()
        {
            var approval = await CreateRegistration();

            await CreateAccount(approval, dateOfBirth: fixture.Create<DateTime>());
            var response = await PostVerifyRegistrationCommand(approval.ApprenticeId, approval.ApprenticeId);

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Sorry, your identity has not been verified, please check your details\"*");
        }

        [Test]
        public async Task Valid_match_creates_apprenticeship()
        {
            var approval = await CreateRegistration();
            await CreateAccount(approval);

            await VerifyRegistration(approval.ApprenticeId, approval.ApprenticeId);

            var apprenticeships = await GetApprenticeships(approval.ApprenticeId);
            apprenticeships.Should().ContainEquivalentOf(new
            {
                approval.ApprenticeId,
                approval.CommitmentsApprenticeshipId,
                //approval.CommitmentsApprovedOn,
            });
        }

        [Test]
        public async Task Cannot_match_twice()
        {
            var approval = await CreateRegistration();
            await CreateAccount(approval);
            await VerifyRegistration(approval.ApprenticeId, approval.ApprenticeId);

            var response = await PostVerifyRegistrationCommand(approval.ApprenticeId, approval.ApprenticeId);

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Registration * is already verified\"*");
        }
    }
}