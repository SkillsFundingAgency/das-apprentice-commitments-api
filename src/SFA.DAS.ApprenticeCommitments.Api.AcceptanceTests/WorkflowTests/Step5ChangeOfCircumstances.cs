using AutoFixture;
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
}
