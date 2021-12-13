﻿using AutoFixture;
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

        [When(@"we change the apprentice's email address")]
        public async Task WhenWeChangeTheApprenticesEmailAddress()
        {
            await _context.Api.Patch($"apprentices/{_apprentice.Id}", _request);
            _context.Api.Response.Should().Be2XXSuccessful();
        }

        [Then(@"an ApprenticeEmailAddressedChangedEvent is published for each apprenticeship")]
        public void ThenAnApprenticeEmailAddressedChangedEventIsPublishedForEachApprenticeship()
        {
           var events =  _context.PublishedNServiceBusEvents
                .Where(x=>x.Event is ApprenticeshipEmailAddressChangedEvent);

           events.Should().ContainEquivalentOf(new
           {
               Event = new
               {
                   ApprenticeId = _apprentice.Id,
                   _revisionForFirstApprenticeship.CommitmentsApprenticeshipId
               }
           });

           events.Should().ContainEquivalentOf(new
           {
               Event = new
               {
                   ApprenticeId = _apprentice.Id,
                   _revisionForSecondApprenticeship.CommitmentsApprenticeshipId
               }
           });
        }
    }
}