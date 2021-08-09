using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateAccountCommand;
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

        [Test, AutoData]
        public void Validates_email(CreateRegistrationCommandValidator sut, CreateRegistrationCommand data)
        {
            data.Email = default;
            var result = sut.TestValidate(data);
            result.ShouldHaveValidationErrorFor(p => p.Email);
        }

        [Test]
        public async Task Cannot_retrieve_missing_registration()
        {
            var response = await client.GetAsync($"registrations/{Guid.NewGuid()}");
            response.Should().Be404NotFound();
        }

        [Test]
        public async Task Can_retrieve_registration()
        {
            var create = await CreateRegistration();
            var registration = await GetRegistration(create.RegistrationId);
            registration.Should().BeEquivalentTo(new
            {
                create.RegistrationId,
                create.DateOfBirth,
                create.Email,
                HasViewedVerification = false,
                HasCompletedVerification = false,
            });
        }
    }

    public class InvitationRework_CreateApprenticeAccount : ApiFixture
    {
        [Test]
        public async Task Validates_command()
        {
            var create = fixture.Build<CreateAccountCommand>()
                .Without(p => p.Email).
                Create();
            var response = await PostCreateAccountCommand(create);
            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*Email*");
        }

        [Test]
        public async Task Cannot_retrieve_missing_apprentice()
        {
            var response = await client.GetAsync($"apprentices/{Guid.NewGuid()}");
            response.Should().Be404NotFound();
        }

        [Test]
        public async Task Can_retrieve_apprentice()
        {
            var approval = fixture.Create<CreateRegistrationCommand>();
            var account = await CreateAccount(approval);
            var apprentice = await GetApprentice(account.ApprenticeId);
            apprentice.Should().BeEquivalentTo(new
            {
                Id = account.ApprenticeId,
                approval.DateOfBirth,
                approval.Email,
                approval.FirstName,
                approval.LastName,
            });
        }

        [Test]
        public async Task Stored_email_address_history()
        {
            var approval = fixture.Create<CreateRegistrationCommand>();

            var account = await CreateAccount(approval);

            var apprentice = await Database.Apprentices
                .Include(x => x.PreviousEmailAddresses)
                .FirstOrDefaultAsync(x => x.Id == account.ApprenticeId);
            apprentice.PreviousEmailAddresses.Should().ContainEquivalentOf(new
            {
                EmailAddress = new MailAddress(account.Email),
            });
        }
    }

    public class InvitationRework_MatchApprenticeshipToApproval : ApiFixture
    {
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

            var response = await PostVerifyRegistrationCommand(approval.RegistrationId, Guid.NewGuid());

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Apprentice * not found\"*");
        }

        [Test]
        public async Task Cannot_match_incorrect_email()
        {
            var approval = await CreateRegistration();

            var account = await CreateAccount(approval, email: fixture.Create<MailAddress>());
            var response = await PostVerifyRegistrationCommand(approval.RegistrationId, account.ApprenticeId);

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Sorry, your identity has not been verified, please check your details\"*");
        }

        [Test]
        public async Task Cannot_match_incorrect_date_of_birth()
        {
            var approval = await CreateRegistration();

            var account = await CreateAccount(approval, dateOfBirth: fixture.Create<DateTime>());
            var response = await PostVerifyRegistrationCommand(approval.RegistrationId, account.ApprenticeId);

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Sorry, your identity has not been verified, please check your details\"*");
        }

        [Test]
        public async Task Valid_match_creates_apprenticeship()
        {
            var approval = await CreateRegistration();
            var account = await CreateAccount(approval);

            await VerifyRegistration(approval.RegistrationId, account.ApprenticeId);

            var apprenticeships = await GetApprenticeships(account.ApprenticeId);
            apprenticeships.Should().ContainEquivalentOf(new
            {
                account.ApprenticeId,
                approval.CommitmentsApprenticeshipId,
                //approval.CommitmentsApprovedOn,
                EmployerCorrect = (bool?)null,
                TrainingProviderCorrect = (bool?)null,
                ApprenticeshipDetailsCorrect = (bool?)null,
                HowApprenticeshipDeliveredCorrect = (bool?)null,
                RolesAndResponsibilitiesCorrect = (bool?)null,
            });
        }

        [Test]
        public async Task Cannot_match_twice()
        {
            var approval = await CreateRegistration();
            var account = await CreateAccount(approval);
            await VerifyRegistration(approval.RegistrationId, account.ApprenticeId);

            var response = await PostVerifyRegistrationCommand(approval.RegistrationId, account.ApprenticeId);

            response
                .Should().Be400BadRequest()
                .And.MatchInContent("*\"Registration * is already verified\"*");
        }
    }

    public class InvitationRework_Confirmations : ApiFixture
    {
        [TestCase("EmployerCorrect", true)]
        [TestCase("EmployerCorrect", false)]
        [TestCase("EmployerCorrect", null)]
        [TestCase("TrainingProviderCorrect", true)]
        [TestCase("TrainingProviderCorrect", false)]
        [TestCase("TrainingProviderCorrect", null)]
        [TestCase("ApprenticeshipDetailsCorrect", true)]
        [TestCase("ApprenticeshipDetailsCorrect", false)]
        [TestCase("ApprenticeshipDetailsCorrect", null)]
        [TestCase("HowApprenticeshipDeliveredCorrect", true)]
        [TestCase("HowApprenticeshipDeliveredCorrect", false)]
        [TestCase("HowApprenticeshipDeliveredCorrect", null)]
        [TestCase("RolesAndResponsibilitiesCorrect", true)]
        [TestCase("RolesAndResponsibilitiesCorrect", false)]
        [TestCase("RolesAndResponsibilitiesCorrect", null)]
        public async Task Can_confirm_new_api(string confirmation, bool? value)
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            object data = confirmation switch
            {
                "EmployerCorrect" => new { EmployerCorrect = value },
                "TrainingProviderCorrect" => new { TrainingProviderCorrect = value },
                "ApprenticeshipDetailsCorrect" => new { ApprenticeshipDetailsCorrect = value },
                "HowApprenticeshipDeliveredCorrect" => new { TrainingProviderCorrect = value },
                "RolesAndResponsibilitiesCorrect" => new { TrainingProviderCorrect = value },
                _ => throw new ArgumentOutOfRangeException(nameof(confirmation)),
            };

            var r4 = await client.PatchValueAsync(
                $"apprentices/{apprenticeship.ApprenticeId}/apprenticeships/{apprenticeship.Id}/revisions/{apprenticeship.CommitmentStatementId}/confirmations",
                data);
            r4.Should().Be2XXSuccessful();

            apprenticeship = (await GetApprenticeships(apprenticeship.ApprenticeId))[0];
            apprenticeship.Should().BeEquivalentTo(data);
        }
    }
}