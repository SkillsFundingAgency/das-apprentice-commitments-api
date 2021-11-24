using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.DTOs;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
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
        private JsonPatchDocument<ApprenticeDto> _request;
        private string _newEmailAddress;

        public ChangeEmailAddressSteps(TestContext context)
        {
            _context = context;
            _newEmailAddress = "NewValidEmail@test.com";
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
        public async Task GivenWeHaveAnExistingApprenticeWithMultipleApprenticeships()
        {
            _apprentice = _fixture.Create<Apprentice>();
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();

            _revisionForFirstApprenticeship = _fixture.Create<Revision>();
            _context.DbContext.Apprenticeships.Add(new Apprenticeship(_revisionForFirstApprenticeship, _apprentice.Id));

            _revisionForSecondApprenticeship = _fixture.Create<Revision>();
            _context.DbContext.Apprenticeships.Add(new Apprenticeship(_revisionForSecondApprenticeship, _apprentice.Id)); 

            _context.DbContext.SaveChanges();
        }

        [Given(@"a ChangeEmailCommand with a valid email address")]
        public void GivenAChangeEmailCommandWithAValidEmailAddress()
        {
            _request = new JsonPatchDocument<ApprenticeDto>().Replace(x => x.Email, _newEmailAddress);
        }

        [Given(@"a ChangeEmailCommand with an invalid email address")]
        public void GivenAChangeEmailCommandWithAnInvalidEmailAddress()
        {
            _request = new JsonPatchDocument<ApprenticeDto>().Replace(x => x.Email, _fixture.Create<long>().ToString());
        }

        [Given("a ChangeEmailCommand with the current email address")]
        public void GivenAChangeEmailCommandWithTheCurrentEmailAddress()
        {
            _request = new JsonPatchDocument<ApprenticeDto>().Replace(x => x.Email, _apprentice.Email.ToString());
        }

        [When(@"we change the apprentice's email address")]
        public async Task WhenWeChangeTheApprenticesEmailAddress()
        {
            await _context.Api.Patch($"apprentices/{_apprentice.Id}", _request);
        }

        [Then(@"the apprentice record is updated")]
        public void ThenTheApprenticeRecordIsCreated()
        {
            _context.DbContext.Apprentices.Should().ContainEquivalentOf(new
            {
                _apprentice.Id,
                Email = new MailAddress(_newEmailAddress),
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
                EmailAddress = new MailAddress(_newEmailAddress),
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