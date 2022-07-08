using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.DTOs;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "GetMyApprenticeship")]
    public class GetMyApprenticeshipSteps
    {
        private readonly TestContext _context;
        private Fixture _fixture = new Fixture();
        private Apprentice _apprentice;
        private Revision _revision;
        private Revision _revisionWithoutFullConfirmation;
        private Revision _newerRevision;
        private Apprenticeship _apprenticeship;

        public GetMyApprenticeshipSteps(TestContext context)
        {
            _context = context;
            _apprentice = _fixture.Build<Apprentice>()
                .With(a => a.TermsOfUseAccepted, true)
                .Create();

            var startDate = new System.DateTime(2000, 01, 01);
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

            _revisionWithoutFullConfirmation = _fixture.Create<Revision>();

            _newerRevision = _fixture.Build<Revision>()
                .Do(a => a.Confirm(new Confirmations
                {
                    TrainingProviderCorrect = true,
                    EmployerCorrect = true,
                    ApprenticeshipDetailsCorrect = true,
                    HowApprenticeshipDeliveredCorrect = true,
                }, DateTimeOffset.UtcNow))
                .Create();
            _apprenticeship = new Apprenticeship(_revision, _apprentice.Id);
        }

        [Given(@"the apprenticeship exists and it's associated with this apprentice")]
        public async Task GivenTheApprenticeshipExistsAndItSAssociatedWithThisApprentice()
        {
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();

            _context.DbContext.Apprenticeships.Add(_apprenticeship);
            await _context.DbContext.SaveChangesAsync();
        }

        [Given(@"many confirmed revisions exist")]
        public async Task GivenManyConfirmedRevisionsExist()
        {
            _apprenticeship
                .Revise(
                _revision.CommitmentsApprenticeshipId,
                _fixture.Create<ApprenticeshipDetails>(),
                _revision.CommitmentsApprovedOn.AddDays(1));

            await _context.DbContext.SaveChangesAsync();
        }

        [Given(@"the apprenticeship exists, but no revision has been confirmed")]
        public async Task GivenTheApprenticeshipExistsButNoRevisionHasBeenConfirmed()
        {
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();

            _context.DbContext.Apprenticeships.Add(new Apprenticeship(_revisionWithoutFullConfirmation, _apprentice.Id));
            await _context.DbContext.SaveChangesAsync();
        }

        [Given("many apprenticeships exists and are associated with this apprentice")]
        public async Task GivenManyApprenticeshipExistsAndAreAssociatedWithThisApprentice()
        {
            _fixture.Register((int i) => _revision.CommitmentsApprovedOn.AddDays(-i));
            
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();

            _context.DbContext.Apprenticeships.Add(new Apprenticeship(_fixture.Create<Revision>(), _apprentice.Id));
            _context.DbContext.Apprenticeships.Add(new Apprenticeship(_fixture.Create<Revision>(), _apprentice.Id));
            _context.DbContext.Apprenticeships.Add(new Apprenticeship(_revision, _apprentice.Id));
            await _context.DbContext.SaveChangesAsync();
        }

        [Given(@"there is no apprenticeship")]
        public void GivenThereIsNoApprenticeship()
        {
        }

        [When(@"we try to retrieve the latest confirmed apprenticeship")]
        public async Task WhenWeTryToRetrieveTheLatestConfirmedApprenticeship()
        {
            await _context.Api.Get($"apprentices/{_apprentice.Id}/apprenticeships/{_revision.ApprenticeshipId}/confirmed/latest");
        }

        [Then(@"the result should return ok")]
        public void ThenTheResultShouldReturnOk()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the response should match the last confirmed apprenticeship values")]
        public async Task ThenTheResponseShouldMatchTheLastConfirmedApprenticeshipValues()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var a = JsonConvert.DeserializeObject<ApprenticeshipDto>(content);
            TheReturnedApprenticeshipMatchesExpectedRevision(a, _revision);
        }

        private void TheReturnedApprenticeshipMatchesExpectedRevision(ApprenticeshipDto a, Revision revision)
        {
            a.Should().NotBeNull();
            a.Id.Should().Be(_revision.ApprenticeshipId);
            a.CommitmentsApprenticeshipId.Should().Be(revision.CommitmentsApprenticeshipId);
            a.EmployerName.Should().Be(revision.Details.EmployerName);
            a.EmployerAccountLegalEntityId.Should().Be(revision.Details.EmployerAccountLegalEntityId);
            a.TrainingProviderName.Should().Be(revision.Details.TrainingProviderName);
            a.TrainingProviderCorrect.Should().Be(revision.TrainingProviderCorrect);
            a.EmployerCorrect.Should().Be(revision.EmployerCorrect);
            a.ApprenticeshipDetailsCorrect.Should().Be(revision.ApprenticeshipDetailsCorrect);
            a.HowApprenticeshipDeliveredCorrect.Should().Be(revision.HowApprenticeshipDeliveredCorrect);
            a.CourseName.Should().Be(revision.Details.Course.Name);
            a.CourseLevel.Should().Be(revision.Details.Course.Level);
            a.CourseOption.Should().Be(revision.Details.Course.Option);
            a.PlannedStartDate.Should().Be(revision.Details.Course.PlannedStartDate);
            a.PlannedEndDate.Should().Be(revision.Details.Course.PlannedEndDate);
            a.CourseDuration.Should().Be(32 + 1); // Duration is inclusive of start and end months
            a.EmploymentEndDate.Should().Be(revision.Details.Course.EmploymentEndDate);
        }

        [Then(@"the result should return NotFound")]
        public void ThenTheResultShouldReturnNotFound()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}