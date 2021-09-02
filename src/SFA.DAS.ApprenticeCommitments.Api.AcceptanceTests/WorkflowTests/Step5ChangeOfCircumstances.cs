using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    class Step5ChangeOfCircumstances : ApiFixture
    {
        [TestCase]
        public async Task Can_update_email_from_ChangeOfCircumstances_before_ther_is_a_matching_account()
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
    }
}
