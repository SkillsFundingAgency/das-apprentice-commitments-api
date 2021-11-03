using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeCommitments.Api.Controllers;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "ChangeEmailAddress")]
    public class ChangeEmailAddressSteps
    {
        private readonly TestContext _context;
        private Fixture _fixture = new Fixture();
        private Apprentice _apprentice;
        private Revision _revisionForFirstApprenticeship;
        private Revision _revisionForSecondApprenticeship;
        private ChangeEmailAddressRequest _command;

        public ChangeEmailAddressSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"we have an existing apprentice")]
        public void GivenWeHaveAnExistingApprentice()
        {
            _apprentice = _fixture.Build<Apprentice>()
                .Without(a => a.TermsOfUseAccepted)
                .Create();

            _context.DbContext.Apprentices.Add(_apprentice);
            _context.DbContext.SaveChanges();
        }

        [Given(@"we have an existing apprentice with multiple apprenticeships")]
        public void GivenWeHaveAnExistingApprenticeWithMultipleApprenticeships()
        {
            _apprentice = _fixture.Create<Apprentice>();

            _revisionForFirstApprenticeship = _fixture.Create<Revision>();
            _apprentice.AddApprenticeship(_revisionForFirstApprenticeship);

            _revisionForSecondApprenticeship = _fixture.Create<Revision>();
            _apprentice.AddApprenticeship(_revisionForSecondApprenticeship);

            _context.DbContext.Apprentices.Add(_apprentice);
            _context.DbContext.SaveChanges();
        }

        [Given(@"a ChangeEmailCommand with a valid email address")]
        public void GivenAChangeEmailCommandWithAValidEmailAddress()
        {
            _command = _fixture
                .Build<ChangeEmailAddressRequest>()
                .With(p => p.Email, (MailAddress adr) => adr.ToString())
                .Create();
        }

        [Given(@"a ChangeEmailCommand with an invalid email address")]
        public void GivenAChangeEmailCommandWithAnInvalidEmailAddress()
        {
            GivenAChangeEmailCommandWithAValidEmailAddress();
            _command.Email = _fixture.Create<long>().ToString();
        }

        [Given("a ChangeEmailCommand with the current email address")]
        public void GivenAChangeEmailCommandWithTheCurrentEmailAddress()
        {
            GivenAChangeEmailCommandWithAValidEmailAddress();
            _command.Email = _apprentice.Email.ToString();
        }

        [When(@"we change the apprentice's email address")]
        public async Task WhenWeChangeTheApprenticesEmailAddress()
        {
            await _context.Api.Post($"apprentices/{_apprentice.Id}/email", _command);
        }

        [Then(@"the apprentice record is updated")]
        public void ThenTheApprenticeRecordIsCreated()
        {
            _context.DbContext.Apprentices.Should().ContainEquivalentOf(new
            {
                _apprentice.Id,
                Email = new MailAddress(_command.Email),
            });
        }

        [Then(@"the apprentice record is not updated")]
        public void ThenTheApprenticeRecordIsNotUpdated()
        {
            _context.DbContext.Apprentices.Should().ContainEquivalentOf(_apprentice);
        }

        [Then(@"the change history is recorded")]
        public void ThenTheChangeHistoryIsRecorded()
        {
            var modified = _context.DbContext
                .Apprentices.Include(x => x.PreviousEmailAddresses)
                .Single(x => x.Id == _apprentice.Id);

            modified.PreviousEmailAddresses.Should().ContainEquivalentOf(new
            {
                EmailAddress = new MailAddress(_command.Email),
            });
        }

        [Then(@"an ApprenticeEmailAddressedChangedEvent is published for each apprenticeship")]
        public void ThenAnApprenticeEmailAddressedChangedEventIsPublishedForEachApprenticeship()
        {
           var messages =  _context.Messages.PublishedMessages
                .Where(x=>x.Message is ApprenticeshipEmailAddressChangedEvent);

           messages.Should().ContainEquivalentOf(new
           {
               Message = new
               {
                   ApprenticeId = _apprentice.Id,
                   _revisionForFirstApprenticeship.CommitmentsApprenticeshipId
               }
           });

           messages.Should().ContainEquivalentOf(new
           {
               Message = new
               {
                   ApprenticeId = _apprentice.Id,
                   _revisionForSecondApprenticeship.CommitmentsApprenticeshipId
               }
           });

        }
    }
}