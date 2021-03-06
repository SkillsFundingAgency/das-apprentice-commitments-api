﻿using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Api.Types;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "RegistrationFirstSeen")]
    public class RegistrationFirstSeenSteps
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly TestContext _context;
        private Registration _registration;
        private RegistrationFirstSeenRequest _request;

        public RegistrationFirstSeenSteps(TestContext context)
        {
            _context = context;
            _registration = _fixture.Create<Registration>();
            _request = _fixture.Create<RegistrationFirstSeenRequest>();
        }

        [Given(@"this is the first time the apprentice has seen the identity flow")]
        public async Task GivenThisIsTheFirstTimeTheApprenticeHasSeenTheIdentityFlow()
        {
            _registration.SetProperty(x => x.FirstViewedOn, null);
            _context.DbContext.Registrations.Add(_registration);
            await _context.DbContext.SaveChangesAsync();
        }

        [Given(@"this is not the first time the apprentice has seen the identity flow")]
        public async Task GivenThisIsNotTheFirstTimeTheApprenticeHasSeenTheIdentityFlow()
        {
            _registration.SetProperty(x => x.FirstViewedOn, _fixture.Create<DateTime>());
            _context.DbContext.Registrations.Add(_registration);
            await _context.DbContext.SaveChangesAsync();
        }

        [When(@"we receive a request to mark registration as been viewed")]
        public async Task WhenWeReceiveARequestToMarkRegistrationAsBeenViewed()
        {
            await _context.Api.Post($"registrations/{_registration.ApprenticeId}/firstseen", _request);
        }

        [Then(@"the response is Accepted")]
        public void ThenTheResponseIsAccepted()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        }

        [Then(@"the registration record is updated")]
        public void ThenTheRegistrationRecordIsUpdated()
        {
            var reg = _context.DbContext.Registrations.FirstOrDefault(x => x.ApprenticeId == _registration.ApprenticeId);
            reg.Should().NotBeNull();
            reg.FirstViewedOn.Should().Be(_request.SeenOn);
        }

        [Then(@"the registration record is not updated")]
        public void ThenTheRegistrationRecordIsNotUpdated()
        {
            var reg = _context.DbContext.Registrations.FirstOrDefault(x => x.ApprenticeId == _registration.ApprenticeId);
            reg.Should().NotBeNull();
            reg.FirstViewedOn.Should().NotBe(_request.SeenOn);
        }
    }
}