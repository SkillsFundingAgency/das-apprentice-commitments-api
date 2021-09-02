using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    class When_personal_change_of_circumstances_before_apprenticeship_is_matched : ApiFixture
    {
        [Test]
        public async Task Updates_personal_details_in_registration()
        {
            var approval = await CreateRegistration();
            var create = fixture.Create<ChangeApprenticeshipCommand>();
            create.CommitmentsApprenticeshipId = approval.CommitmentsApprenticeshipId;

            await ChangeOfCircumstances(create);

            var registration = Database.Registrations.Find(approval.RegistrationId);
            registration.Should().BeEquivalentTo(new
            {
                approval.RegistrationId,
                create.FirstName,
                create.LastName,
                create.DateOfBirth,
                Email = new MailAddress(create.Email),
            });
        }

        [Test]
        public async Task Triggers_ApprenticeshipRegisteredEvent()
        {
            var approval = await CreateRegistration();
            var create = fixture.Create<ChangeApprenticeshipCommand>();
            create.CommitmentsApprenticeshipId = approval.CommitmentsApprenticeshipId;
            Reset();

            await ChangeOfCircumstances(create);

            Messages.PublishedMessages.Should().ContainEquivalentOf(new
            {
                Message = new ApprenticeshipRegisteredEvent
                {
                    RegistrationId = approval.RegistrationId,
                }
            });
        }
    }

    class When_change_of_circumstances_includes_email_belonging_to_matched_account : ApiFixture
    {
        [Test]
        public async Task Updates_personal_details_in_registration()
        {
            var approval = await CreateRegistration();
            var create = fixture.Create<ChangeApprenticeshipCommand>();
            create.CommitmentsApprenticeshipId = approval.CommitmentsApprenticeshipId;

            await ChangeOfCircumstances(create);

            var registration = Database.Registrations.Find(approval.RegistrationId);
            registration.Should().BeEquivalentTo(new
            {
                approval.RegistrationId,
                create.FirstName,
                create.LastName,
                create.DateOfBirth,
                Email = new MailAddress(create.Email),
            });
        }

        [Test, AutoData]
        public async Task Does_not_trigger_ApprenticeshipRegisteredEvent(MailAddress existingEmail)
        {
            await CreateVerifiedApprenticeship(email: existingEmail);
            var approval = await CreateRegistration();
            var command = fixture.Create<ChangeApprenticeshipCommand>();
            command.CommitmentsApprenticeshipId = approval.CommitmentsApprenticeshipId;
            command.Email = existingEmail.ToString();
            Reset();

            await ChangeOfCircumstances(command);

            Messages.PublishedMessages.Should().BeEmpty();
        }
    }
}
