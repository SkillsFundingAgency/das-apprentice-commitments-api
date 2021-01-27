using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "HealthCheck")]
    public class HealthCheckSteps
    {
        private readonly TestContext _context;

        public HealthCheckSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"the api has started")]
        public void GivenTheApiHasStarted()
        {
        }

        [Given(@"the database is offline")]
        public void GivenTheDatabaseIsOffline()
        {
        }

        [When(@"the ping endpoint is called")]
        public async Task WhenThePingEndpointIsCalled()
        {
            await _context.Api.Get("ping");
        }

        [When(@"the health endpoint is called")]
        public async Task WhenTheHealthEndpointIsCalled()
        {
            await _context.Api.Get("health");
        }

        [Then(@"the result should be return okay")]
        public void ThenTheResultShouldBeReturnOkay()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the result should not be healthy")]
        public async Task ThenTheResultShouldNotBeHealthy()
        {
            var response = await _context.Api.Response.Content.ReadAsStringAsync();

            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }
    }
}
