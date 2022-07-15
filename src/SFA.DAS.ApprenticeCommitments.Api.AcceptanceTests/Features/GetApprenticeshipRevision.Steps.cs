using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "GetApprenticeshipRevision")]
    public class GetApprenticeshipRevisionSteps
    {
        private readonly TestContext _context;
        private readonly Fixture _fixture = new Fixture();
        private readonly Apprentice _apprentice;
        private readonly Revision _revision;
        private Revision _newerRevision;

        public GetApprenticeshipRevisionSteps(TestContext context)
        {
            _context = context;
            _apprentice = _fixture.Build<Apprentice>()
                .With(e => e.TermsOfUseAccepted, true)
                .Create();

            var startDate = new DateTime(2000, 01, 01);
            _fixture.Register(() => new CourseDetails(
                _fixture.Create("CourseName"), 1, null,
                startDate, startDate.AddMonths(32), 33, startDate.AddMonths(5)));

            _revision = _fixture.Build<Revision>()
                .Do(a => a.Confirm(new Confirmations
                {
                    EmployerCorrect = true,
                    TrainingProviderCorrect = true,
                    ApprenticeshipDetailsCorrect = true,
                    RolesAndResponsibilitiesConfirmations = RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed | 
                                                            RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed | 
                                                            RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed,
                    HowApprenticeshipDeliveredCorrect = true,
                    ApprenticeshipCorrect = true,
                }, DateTime.Now))
                .Create();

            _newerRevision = _fixture.Build<Revision>()
                .Do(a => a.Confirm(new Confirmations
                {
                    TrainingProviderCorrect = true,
                    EmployerCorrect = true,
                    ApprenticeshipDetailsCorrect = true,
                    HowApprenticeshipDeliveredCorrect = true,
                }, DateTimeOffset.UtcNow))
                .Create();
        }

        [Given("the apprenticeship exists and it's associated with this apprentice")]
        public async Task GivenTheApprenticeshipExistsAndItSAssociatedWithThisApprentice()
        {
            _context.DbContext.Apprenticeships.Add(new Apprenticeship(_revision, _apprentice.Id));
            await _context.DbContext.SaveChangesAsync();
        }

        [Given("there is no apprenticeship revision")]
        public void GivenThereIsNoApprenticeshipRevision()
        {
        }

        [When("we try to retrieve the apprenticeship revision")]
        public async Task WhenWeTryToRetrieveTheApprenticeship()
        {
            await _context.Api.Get($"apprentices/{_apprentice.Id}/apprenticeships/{_revision.ApprenticeshipId}/revisions/{_revision.Id}");
        }

        [Then("the result should return ok")]
        public void ThenTheResultShouldReturnOk()
        {
            _context.Api.Response.Should().Be200Ok();
        }

        [Then("the response should match the expected apprenticeship values")]
        public void ThenTheResponseShouldMatchTheExpectedApprenticeshipValues()
        {
            var apprenticeship = _context.DbContext.Apprenticeships
                .Include(x => x.Revisions)
                .Where(x => x.Id == _revision.ApprenticeshipId)
                .First();

            _context.Api.Response
                .Should().BeAs(new
                {
                        RevisionId = _revision.Id,
                        _revision.LastViewed,
                        _revision.Details.EmployerName,
                        _revision.Details.EmployerAccountLegalEntityId,
                        _revision.Details.TrainingProviderId,
                        _revision.Details.TrainingProviderName,
                        CourseName = _revision.Details.Course.Name,
                        CourseLevel = _revision.Details.Course.Level,
                        _revision.Details.Course.PlannedStartDate,
                        _revision.Details.Course.PlannedEndDate,
                        _revision.EmployerCorrect,
                        _revision.TrainingProviderCorrect,
                        _revision.ApprenticeshipDetailsCorrect,
                        _revision.HowApprenticeshipDeliveredCorrect,
                        _revision.RolesAndResponsibilitiesConfirmations,
                        CourseDuration = 32 + 1, // Duration is inclusive of start and end months
                });
        }

        [Then("the result should return NotFound")]
        public void ThenTheResultShouldReturnNotFound()
        {
            _context.Api.Response.Should().Be404NotFound();
        }
    }
}