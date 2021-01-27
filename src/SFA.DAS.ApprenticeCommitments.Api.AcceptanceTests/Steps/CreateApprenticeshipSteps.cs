using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "CreateApprenticeship")]
    public class CreateApprenticeshipSteps
    {
        private readonly TestContext _context;
        private CreateRegistrationCommand _createApprenticeshipRequest; 

        public CreateApprenticeshipSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"we have an invalid apprenticeship request")]
        public void GivenWeHaveAnInvalidApprenticeshipRequest()
        {
            _createApprenticeshipRequest = new CreateRegistrationCommand();
        }

        [Given(@"we have a valid apprenticeship request")]
        public void GivenWeHaveAValidApprenticeshipRequest()
        {
            _createApprenticeshipRequest = new CreateRegistrationCommand
            {
                RegistrationId = Guid.NewGuid(),
                ApprenticeshipId = 1233,
                Email = "paul@fff.com"
            };
        }

        [When(@"the apprenticeship is posted")]
        public async Task WhenTheApprenticeshipIsPosted()
        {
            await _context.Api.Post("apprenticeships", _createApprenticeshipRequest);
        }

        [Then(@"the result should return bad request")]
        public void ThenTheResultShouldReturnBadRequest()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Then(@"the content should contain error list")]
        public async Task ThenTheContentShouldContainErrorList()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();

            var errors = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            errors.Count.Should().BeGreaterOrEqualTo(1);
        }

        [Then(@"the result should return accepted")]
        public void ThenTheResultShouldReturnAccepted()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        }

        [Then(@"the registration exists in database")]
        public void ThenTheRegistrationExistsInDatabase()
        {
            var registration = _context.DbContext.Registrations.FirstOrDefault();
            registration.Should().NotBeNull();
            registration.Email.Should().Be(_createApprenticeshipRequest.Email);
            registration.ApprenticeshipId.Should().Be(_createApprenticeshipRequest.ApprenticeshipId);
        }
    }
}
