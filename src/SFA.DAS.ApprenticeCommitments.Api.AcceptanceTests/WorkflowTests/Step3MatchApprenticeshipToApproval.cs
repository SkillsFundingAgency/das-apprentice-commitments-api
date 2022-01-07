using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    public class Step3MatchApprenticeshipToApproval : ApiFixture
    {
        [Test]
        public async Task Cannot_find_registration_that_doesnt_exist()
        {
            var response = await PostVerifyRegistrationCommand(Guid.NewGuid(), Guid.NewGuid(), "", DateTime.UtcNow);

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Registration for Apprentice * not found\"*");
        }

        [Test]
        public async Task Record_succsessful_match()
        {
            var approval = await CreateRegistration();
            var account = await CreateAccount(approval);

            var response = await PostVerifyRegistrationCommand(approval, account.ApprenticeId);
            response.Should().Be2XXSuccessful();

            Database.ApprenticeshipMatchAttempts.Should().ContainEquivalentOf(new
            {
                approval.RegistrationId,
                account.ApprenticeId,
                Status = ApprenticeshipMatchAttemptStatus.Succeeded,
            });
        }

        [Test]
        public async Task Can_match_incorrect_email()
        {
            var approval = await CreateRegistration();

            var account = await CreateAccount(approval, email: fixture.Create<MailAddress>());
            var response = await PostVerifyRegistrationCommand(approval, account.ApprenticeId);

            response.Should().Be2XXSuccessful();
        }

        [Test]
        public async Task Cannot_match_incorrect_date_of_birth()
        {
            var approval = await CreateRegistration();

            var account = await CreateAccount(approval, dateOfBirth: fixture.Create<DateTime>());
            var response = await PostVerifyRegistrationCommand(approval.RegistrationId, account.ApprenticeId, approval.LastName, DateTime.UtcNow);

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Sorry, your identity has not been verified, please check your details\"*");

            Database.ApprenticeshipMatchAttempts.Should().ContainEquivalentOf(new
            {
                approval.RegistrationId,
                account.ApprenticeId,
                Status = ApprenticeshipMatchAttemptStatus.MismatchedDateOfBirth,
            });
        }

        [Test]
        public async Task Valid_match_creates_apprenticeship()
        {
            var approval = await CreateRegistration();
            var account = await CreateAccount(approval);

            await VerifyRegistration(approval, account.ApprenticeId);

            var apprenticeships = await GetApprenticeships(account.ApprenticeId);
            apprenticeships.Should().ContainEquivalentOf(new
            {
                account.ApprenticeId,
                approval.CommitmentsApprenticeshipId,
                EmployerCorrect = (bool?)null,
                TrainingProviderCorrect = (bool?)null,
                ApprenticeshipDetailsCorrect = (bool?)null,
                HowApprenticeshipDeliveredCorrect = (bool?)null,
                RolesAndResponsibilitiesConfirmations = RolesAndResponsibilitiesConfirmations.None,
            });
        }

        [Test]
        public async Task Second_match_for_same_apprentice_is_accepted()
        {
            var approval = await CreateRegistration();
            var account = await CreateAccount(approval);
            await VerifyRegistration(approval, account.ApprenticeId);

            var response = await PostVerifyRegistrationCommand(approval, account.ApprenticeId);

            response.Should().Be2XXSuccessful();
        }

        [Test]
        public async Task Cannot_match_a_different_apprentice_against_already_matched_registration()
        {
            var approval = await CreateRegistration();
            var firstAccount = await CreateAccount(approval);
            await VerifyRegistration(approval, firstAccount.ApprenticeId);
            var secondAccount = await CreateAccount();

            var response = await PostVerifyRegistrationCommand(approval, secondAccount.ApprenticeId);

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Registration * is already verified\"*");
        }
    }
}