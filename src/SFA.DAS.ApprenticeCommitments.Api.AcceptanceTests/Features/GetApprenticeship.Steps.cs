using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "GetApprenticeship")]
    public class GetApprenticeshipSteps
    {
        private readonly TestContext _context;
        private Fixture _fixture = new Fixture();
        private Guid _apprenticeId;
        private Revision _revision;
        private Revision _newerRevision;
        private long _apprenticeshipId;

        public GetApprenticeshipSteps(TestContext context)
        {
            _context = context;
            _apprenticeId = _fixture.Create<Guid>();

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

        [Given(@"the apprenticeship exists and it's associated with this apprentice")]
        public async Task GivenTheApprenticeshipExistsAndItSAssociatedWithThisApprentice()
        {
            var apprenticeship = new Apprenticeship(_revision, _apprenticeId);
            _context.DbContext.Apprenticeships.Add(apprenticeship);
            await _context.DbContext.SaveChangesAsync();
            _apprenticeshipId = apprenticeship.Id;
        }

        [Given("many apprenticeships exists and are associated with this apprentice")]
        public async Task GivenManyApprenticeshipExistsAndAreAssociatedWithThisApprentice()
        {
            // Ensure previous approvals happened before the one we will later assert on, so 
            // the GetApprenticeship feature finds our one as the latest approval
            _fixture.Register((int i) => _revision.CommitmentsApprovedOn.AddDays(-i));
            
            var apprenticeship = new Apprenticeship(_revision, _apprenticeId);
            _context.DbContext.Apprenticeships.Add(new Apprenticeship(_fixture.Create<Revision>(), _apprenticeId));
            _context.DbContext.Apprenticeships.Add(new Apprenticeship(_fixture.Create<Revision>(), _apprenticeId));
            _context.DbContext.Apprenticeships.Add(apprenticeship);
            await _context.DbContext.SaveChangesAsync();
            _apprenticeshipId = apprenticeship.Id;
        }

        [Given("the apprenticeships exists, has many revisions, and is associated with this apprentice")]
        public async Task GivenTheApprenticeshipsExistsHasManyCommitmentRevisionsAndIsAssociatedWithThisApprentice()
        {
            var apprenticeship = new Apprenticeship(_revision, _apprenticeId);
            _context.DbContext.Apprenticeships.Add(apprenticeship);

            apprenticeship
                .Revise(
                _revision.CommitmentsApprenticeshipId,
                _fixture.Create<ApprenticeshipDetails>(),
                _revision.CommitmentsApprovedOn.AddDays(1));

            _newerRevision = apprenticeship.Revisions.Last();
            _newerRevision.Confirm(new Confirmations
            {
                TrainingProviderCorrect = true,
                EmployerCorrect = true,
                ApprenticeshipDetailsCorrect = true,
                HowApprenticeshipDeliveredCorrect = true,
            }, DateTimeOffset.UtcNow);

            await _context.DbContext.SaveChangesAsync();
            _apprenticeshipId = apprenticeship.Id;
        }

        [Given("the apprenticeships exists, has many revisions, and a previous revision has been confirmed")]
        public async Task GivenTheApprenticeshipsExistsHasManyRevisionsAndAPreviousRevisionHasBeenConfirmed()
        {
            _revision.Confirm(new Confirmations
            {
                TrainingProviderCorrect = true,
                EmployerCorrect = true,
                ApprenticeshipDetailsCorrect = true,
                HowApprenticeshipDeliveredCorrect = true
            }, DateTimeOffset.UtcNow.AddDays(-1));

            var apprenticeship = new Apprenticeship(_revision, _apprenticeId);
            _context.DbContext.Apprenticeships.Add(apprenticeship);

            apprenticeship
                .Revise(
                    _revision.CommitmentsApprenticeshipId,
                    _fixture.Create<ApprenticeshipDetails>(),
                    _revision.CommitmentsApprovedOn.AddDays(1));

            _newerRevision = apprenticeship.Revisions.Last();
            await _context.DbContext.SaveChangesAsync();
            _apprenticeshipId = apprenticeship.Id;
        }

        [Given(@"the apprenticeships exists, has many unconfirmed revisions")]
        public async Task GivenTheApprenticeshipsExistsHasManyUnconfirmedRevisions()
        {
            var unapprovedRevision = new Revision(_revision.CommitmentsApprenticeshipId, _revision.CommitmentsApprovedOn, _fixture.Create<ApprenticeshipDetails>());

            var apprenticeship = new Apprenticeship(unapprovedRevision, _apprenticeId);
            _context.DbContext.Apprenticeships.Add(apprenticeship);

            apprenticeship
                .Revise(
                    _revision.CommitmentsApprenticeshipId,
                    _fixture.Create<ApprenticeshipDetails>(),
                    _revision.CommitmentsApprovedOn.AddDays(1));

            _newerRevision = apprenticeship.Revisions.Last();
            await _context.DbContext.SaveChangesAsync();
            _apprenticeshipId = apprenticeship.Id;
        }

        [Given(@"there is no apprenticeship")]
        public void GivenThereIsNoApprenticeship()
        {
        }

        [Given(@"the apprenticeship exists, but it's associated with another apprentice")]
        public async Task GivenTheApprenticeshipExistsButItSAssociatedWithAnotherApprentice()
        {
            var anotherApprenticeId = _fixture.Create<Guid>();

            _context.DbContext.Apprenticeships.Add(new Apprenticeship(_revision, anotherApprenticeId));
            await _context.DbContext.SaveChangesAsync();
        }

        [When(@"we try to retrieve the apprenticeship")]
        public async Task WhenWeTryToRetrieveTheApprenticeship()
        {
            await _context.Api.Get($"apprentices/{_apprenticeId}/apprenticeships/{_apprenticeshipId}");
        }

        [Then(@"the result should return ok")]
        public void ThenTheResultShouldReturnOk()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the response should match the expected apprenticeship values")]
        public async Task ThenTheResponseShouldMatchTheExpectedApprenticeshipValues()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var a = JsonConvert.DeserializeObject<ApprenticeshipDto>(content);
            a.Should().NotBeNull();
            a.Id.Should().Be(_revision.ApprenticeshipId);
            a.CommitmentsApprenticeshipId.Should().Be(_revision.CommitmentsApprenticeshipId);
            a.EmployerName.Should().Be(_revision.Details.EmployerName);
            a.EmployerAccountLegalEntityId.Should().Be(_revision.Details.EmployerAccountLegalEntityId);
            a.TrainingProviderName.Should().Be(_revision.Details.TrainingProviderName);
            a.TrainingProviderCorrect.Should().Be(_revision.TrainingProviderCorrect);
            a.EmployerCorrect.Should().Be(_revision.EmployerCorrect);
            a.ApprenticeshipDetailsCorrect.Should().Be(_revision.ApprenticeshipDetailsCorrect);
            a.HowApprenticeshipDeliveredCorrect.Should().Be(_revision.HowApprenticeshipDeliveredCorrect);
            a.CourseName.Should().Be(_revision.Details.Course.Name);
            a.CourseLevel.Should().Be(_revision.Details.Course.Level);
            a.CourseOption.Should().Be(_revision.Details.Course.Option);
            a.PlannedStartDate.Should().Be(_revision.Details.Course.PlannedStartDate);
            a.PlannedEndDate.Should().Be(_revision.Details.Course.PlannedEndDate);
            a.CourseDuration.Should().Be(32 + 1); // Duration is inclusive of start and end months
            a.EmploymentEndDate.Should().Be(_revision.Details.Course.EmploymentEndDate);
        }

        [Then(@"the response should match the newer apprenticeship values")]
        public async Task ThenTheResponseShouldMatchTheNewerApprenticeshipValues()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var a = JsonConvert.DeserializeObject<ApprenticeshipDto>(content);
            a.Should().NotBeNull();
            a.Id.Should().Be(_newerRevision.ApprenticeshipId);
            a.CommitmentsApprenticeshipId.Should().Be(_newerRevision.CommitmentsApprenticeshipId);
            a.EmployerName.Should().Be(_newerRevision.Details.EmployerName);
            a.EmployerAccountLegalEntityId.Should().Be(_newerRevision.Details.EmployerAccountLegalEntityId);
            a.TrainingProviderName.Should().Be(_newerRevision.Details.TrainingProviderName);
            a.TrainingProviderCorrect.Should().Be(_newerRevision.TrainingProviderCorrect);
            a.EmployerCorrect.Should().Be(_newerRevision.EmployerCorrect);
            a.ApprenticeshipDetailsCorrect.Should().Be(_newerRevision.ApprenticeshipDetailsCorrect);
            a.HowApprenticeshipDeliveredCorrect.Should().Be(_newerRevision.HowApprenticeshipDeliveredCorrect);
            a.CourseName.Should().Be(_newerRevision.Details.Course.Name);
            a.CourseLevel.Should().Be(_newerRevision.Details.Course.Level);
            a.CourseOption.Should().Be(_revision.Details.Course.Option);
            a.PlannedStartDate.Should().Be(_newerRevision.Details.Course.PlannedStartDate);
            a.PlannedEndDate.Should().Be(_newerRevision.Details.Course.PlannedEndDate);
            a.CourseDuration.Should().Be(32 + 1); // Duration is inclusive of start and end months
        }

        [Then(@"the response should show apprenticeship has been confirmed at least once")]
        public async Task ThenTheResponseShouldShowApprenticeshipHasBeenConfirmedAtLeastOnce()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var a = JsonConvert.DeserializeObject<ApprenticeshipDto>(content);
            a.Should().NotBeNull();
            a.HasBeenConfirmedAtLeastOnce.Should().BeTrue();
        }

        [Then(@"the response should show apprenticeship has never been confirmed")]
        public async Task ThenTheResponseShouldShowApprenticeshipHasNeverBeenConfirmed()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var a = JsonConvert.DeserializeObject<ApprenticeshipDto>(content);
            a.Should().NotBeNull();
            a.HasBeenConfirmedAtLeastOnce.Should().BeFalse();
        }

        [Then("all revisions should have the same apprenticeship ID")]
        public void ThenAllCommitmentRevisionsShouldHaveTheSameApprenticeshipID()
        {
            var apprentice = _context.DbContext.Apprenticeships.Where(x => x.ApprenticeId == _apprenticeId);
            apprentice.SelectMany(x => x.Revisions)
                .Should().NotBeEmpty()
                .And.OnlyContain(a => a.ApprenticeshipId == _revision.ApprenticeshipId);
        }

        [Then(@"the result should return NotFound")]
        public void ThenTheResultShouldReturnNotFound()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}