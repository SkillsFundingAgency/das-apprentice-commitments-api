using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System;
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
        private Apprentice _apprentice;
        private Apprenticeship _apprenticeship;

        public GetApprenticeshipSteps(TestContext context)
        {
            _context = context;
            _apprentice = _fixture.Build<Apprentice>().Create();

            var startDate = new System.DateTime(2000, 01, 01);
            _fixture.Register(() => new CourseDetails(
                _fixture.Create("CourseName"), 1, null,
                startDate, startDate.AddMonths(32)));

            _apprenticeship = _fixture.Build<Apprenticeship>()
                .Do(a => a.ConfirmTrainingProvider(true))
                .Do(a => a.ConfirmEmployer(true))
                .Do(a => a.ConfirmApprenticeshipDetails(true))
                .Do(a => a.ConfirmHowApprenticeshipWillBeDelivered(true))
                .Create();
        }

        [Given(@"the apprenticeship exists and it's associated with this apprentice")]
        public async Task GivenTheApprenticeshipExistsAndItSAssociatedWithThisApprentice()
        {
            _apprentice.AddApprenticeship(_apprenticeship);
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();
        }

        [Given("many apprenticeships exists and are associated with this apprentice")]
        public async Task GivenManyApprenticeshipExistsAndAreAssociatedWithThisApprentice()
        {
            _apprentice.AddApprenticeship(_fixture.Create<Apprenticeship>());
            _apprentice.AddApprenticeship(_fixture.Create<Apprenticeship>());
            _apprentice.AddApprenticeship(_apprenticeship);
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();
        }

        [Given("the apprenticeships exists, has many commitment statements, and is associated with this apprentice")]
        public async Task GivenTheApprenticeshipsExistsHasManyCommitmentStatementsAndIsAssociatedWithThisApprentice()
        {
            _apprentice.AddApprenticeship(_apprenticeship);
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();

            _apprenticeship = _apprenticeship.RenewCommitment(
                _fixture.Create<ApprenticeshipDetails>(),
                _apprenticeship.ApprovedOn.AddDays(1));
            _apprentice.AddApprenticeship(_apprenticeship);
            await _context.DbContext.SaveChangesAsync();
        }


        [Given(@"there is no apprenticeship")]
        public void GivenThereIsNoApprenticeship()
        {
        }

        [Given(@"the apprenticeship exists, but it's associated with another apprentice")]
        public async Task GivenTheApprenticeshipExistsButItSAssociatedWithAnotherApprentice()
        {
            var anotherApprentice = _fixture.Create<Apprentice>();
            anotherApprentice.AddApprenticeship(_apprenticeship);

            _context.DbContext.Apprentices.Add(anotherApprentice);
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();
        }

        [When(@"we try to retrieve the apprenticeship")]
        public async Task WhenWeTryToRetrieveTheApprenticeship()
        {
            await _context.Api.Get($"apprentices/{_apprentice.Id}/apprenticeships/{_apprenticeship.ApprenticeshipId}");
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
            a.Id.Should().Be(_apprenticeship.ApprenticeshipId);
            a.CommitmentsApprenticeshipId.Should().Be(_apprenticeship.CommitmentsApprenticeshipId);
            a.EmployerName.Should().Be(_apprenticeship.Details.EmployerName);
            a.EmployerAccountLegalEntityId.Should().Be(_apprenticeship.Details.EmployerAccountLegalEntityId);
            a.TrainingProviderName.Should().Be(_apprenticeship.Details.TrainingProviderName);
            a.TrainingProviderCorrect.Should().Be(_apprenticeship.TrainingProviderCorrect);
            a.EmployerCorrect.Should().Be(_apprenticeship.EmployerCorrect);
            a.ApprenticeshipDetailsCorrect.Should().Be(_apprenticeship.ApprenticeshipDetailsCorrect);
            a.HowApprenticeshipDeliveredCorrect.Should().Be(_apprenticeship.HowApprenticeshipDeliveredCorrect);
            a.CourseName.Should().Be(_apprenticeship.Details.Course.Name);
            a.CourseLevel.Should().Be(_apprenticeship.Details.Course.Level);
            a.CourseOption.Should().Be(_apprenticeship.Details.Course.Option);
            a.PlannedStartDate.Should().Be(_apprenticeship.Details.Course.PlannedStartDate);
            a.PlannedEndDate.Should().Be(_apprenticeship.Details.Course.PlannedEndDate);
            a.DurationInMonths.Should().Be(32 + 1); // Duration is inclusive of start and end months
        }

        [Then("all commitment statements should have the same apprenticeship ID")]
        public async Task ThenAllCommitmentStatementsShouldHaveTheSameApprenticeshipID()
        {
            var apprentice = await _context.DbContext.Apprentices.FindAsync(_apprentice.Id);
            apprentice.Apprenticeships
                .Should().NotBeEmpty()
                .And.OnlyContain(a => a.ApprenticeshipId == _apprenticeship.ApprenticeshipId);
        }

        [Then(@"the result should return NotFound")]
        public void ThenTheResultShouldReturnNotFound()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}