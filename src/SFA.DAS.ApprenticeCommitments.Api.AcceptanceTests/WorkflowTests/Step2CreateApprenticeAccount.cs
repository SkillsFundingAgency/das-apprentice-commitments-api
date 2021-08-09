using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateAccountCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    public class Step2CreateApprenticeAccount : ApiFixture
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
}