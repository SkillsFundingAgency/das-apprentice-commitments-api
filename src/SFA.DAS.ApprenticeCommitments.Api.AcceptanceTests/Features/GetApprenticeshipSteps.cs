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
        private Apprentice _apprentice;
        private CommitmentStatement _commitmentStatement;
        private CommitmentStatement _newerCommitmentStatement;

        public GetApprenticeshipSteps(TestContext context)
        {
            _context = context;
            _apprentice = _fixture.Build<Apprentice>().Create();

            var startDate = new System.DateTime(2000, 01, 01);
            _fixture.Register(() => new CourseDetails(
                _fixture.Create("CourseName"), 1, null,
                startDate, startDate.AddMonths(32)));

            _commitmentStatement = _fixture.Build<CommitmentStatement>()
                .Do(a => a.Confirm(new Confirmations
                {
                    EmployerCorrect = true,
                    TrainingProviderCorrect = true,
                    ApprenticeshipDetailsCorrect = true,
                    RolesAndResponsibilitiesCorrect = true,
                    HowApprenticeshipDeliveredCorrect = true,
                    ApprenticeshipCorrect = true,
                }, DateTime.Now))
                .Create();

            _newerCommitmentStatement = _fixture.Build<CommitmentStatement>()
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
            _apprentice.AddApprenticeship(_commitmentStatement);
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();
        }

        [Given("many apprenticeships exists and are associated with this apprentice")]
        public async Task GivenManyApprenticeshipExistsAndAreAssociatedWithThisApprentice()
        {
            // Ensure previous approvals happened before the one we will later assert on, so 
            // the GetApprenticeship feature finds our one as the latest approval
            _fixture.Register((int i) => _commitmentStatement.CommitmentsApprovedOn.AddDays(-i));
            
            _apprentice.AddApprenticeship(_fixture.Create<CommitmentStatement>());
            _apprentice.AddApprenticeship(_fixture.Create<CommitmentStatement>());
            _apprentice.AddApprenticeship(_commitmentStatement);
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();
        }

        [Given("the apprenticeships exists, has many commitment statements, and is associated with this apprentice")]
        public async Task GivenTheApprenticeshipsExistsHasManyCommitmentStatementsAndIsAssociatedWithThisApprentice()
        {
            _apprentice.AddApprenticeship(_commitmentStatement);
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();

            _apprentice.Apprenticeships.First().RenewCommitment(
                _commitmentStatement.CommitmentsApprenticeshipId,
                _fixture.Create<ApprenticeshipDetails>(),
                _commitmentStatement.CommitmentsApprovedOn.AddDays(1));

            _newerCommitmentStatement = _apprentice.Apprenticeships.First().CommitmentStatements.Last();
            _newerCommitmentStatement.Confirm(new Confirmations
            {
                TrainingProviderCorrect = true,
                EmployerCorrect = true,
                ApprenticeshipDetailsCorrect = true,
                HowApprenticeshipDeliveredCorrect = true,
            }, DateTimeOffset.UtcNow);

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
            anotherApprentice.AddApprenticeship(_commitmentStatement);

            _context.DbContext.Apprentices.Add(anotherApprentice);
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();
        }

        [When(@"we try to retrieve the apprenticeship")]
        public async Task WhenWeTryToRetrieveTheApprenticeship()
        {
            await _context.Api.Get($"apprentices/{_apprentice.Id}/apprenticeships/{_commitmentStatement.ApprenticeshipId}");
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
            a.Id.Should().Be(_commitmentStatement.ApprenticeshipId);
            a.CommitmentsApprenticeshipId.Should().Be(_commitmentStatement.CommitmentsApprenticeshipId);
            a.EmployerName.Should().Be(_commitmentStatement.Details.EmployerName);
            a.EmployerAccountLegalEntityId.Should().Be(_commitmentStatement.Details.EmployerAccountLegalEntityId);
            a.TrainingProviderName.Should().Be(_commitmentStatement.Details.TrainingProviderName);
            a.TrainingProviderCorrect.Should().Be(_commitmentStatement.TrainingProviderCorrect);
            a.EmployerCorrect.Should().Be(_commitmentStatement.EmployerCorrect);
            a.ApprenticeshipDetailsCorrect.Should().Be(_commitmentStatement.ApprenticeshipDetailsCorrect);
            a.HowApprenticeshipDeliveredCorrect.Should().Be(_commitmentStatement.HowApprenticeshipDeliveredCorrect);
            a.CourseName.Should().Be(_commitmentStatement.Details.Course.Name);
            a.CourseLevel.Should().Be(_commitmentStatement.Details.Course.Level);
            a.CourseOption.Should().Be(_commitmentStatement.Details.Course.Option);
            a.PlannedStartDate.Should().Be(_commitmentStatement.Details.Course.PlannedStartDate);
            a.PlannedEndDate.Should().Be(_commitmentStatement.Details.Course.PlannedEndDate);
            a.DurationInMonths.Should().Be(32 + 1); // Duration is inclusive of start and end months
        }

        [Then(@"the response should match the newer apprenticeship values")]
        public async Task ThenTheResponseShouldMatchTheNewerApprenticeshipValues()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var a = JsonConvert.DeserializeObject<ApprenticeshipDto>(content);
            a.Should().NotBeNull();
            a.Id.Should().Be(_newerCommitmentStatement.ApprenticeshipId);
            a.CommitmentsApprenticeshipId.Should().Be(_newerCommitmentStatement.CommitmentsApprenticeshipId);
            a.EmployerName.Should().Be(_newerCommitmentStatement.Details.EmployerName);
            a.EmployerAccountLegalEntityId.Should().Be(_newerCommitmentStatement.Details.EmployerAccountLegalEntityId);
            a.TrainingProviderName.Should().Be(_newerCommitmentStatement.Details.TrainingProviderName);
            a.TrainingProviderCorrect.Should().Be(_newerCommitmentStatement.TrainingProviderCorrect);
            a.EmployerCorrect.Should().Be(_newerCommitmentStatement.EmployerCorrect);
            a.ApprenticeshipDetailsCorrect.Should().Be(_newerCommitmentStatement.ApprenticeshipDetailsCorrect);
            a.HowApprenticeshipDeliveredCorrect.Should().Be(_newerCommitmentStatement.HowApprenticeshipDeliveredCorrect);
            a.CourseName.Should().Be(_newerCommitmentStatement.Details.Course.Name);
            a.CourseLevel.Should().Be(_newerCommitmentStatement.Details.Course.Level);
            a.CourseOption.Should().Be(_commitmentStatement.Details.Course.Option);
            a.PlannedStartDate.Should().Be(_newerCommitmentStatement.Details.Course.PlannedStartDate);
            a.PlannedEndDate.Should().Be(_newerCommitmentStatement.Details.Course.PlannedEndDate);
            a.DurationInMonths.Should().Be(32 + 1); // Duration is inclusive of start and end months
        }



        [Then("all commitment statements should have the same apprenticeship ID")]
        public async Task ThenAllCommitmentStatementsShouldHaveTheSameApprenticeshipID()
        {
            var apprentice = await _context.DbContext.Apprentices.FindAsync(_apprentice.Id);
            apprentice.Apprenticeships.SelectMany(x => x.CommitmentStatements)
                .Should().NotBeEmpty()
                .And.OnlyContain(a => a.ApprenticeshipId == _commitmentStatement.ApprenticeshipId);
        }

        [Then(@"the result should return NotFound")]
        public void ThenTheResultShouldReturnNotFound()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}