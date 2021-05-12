using AutoFixture;
using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "ChangeApprenticeship")]

    public class ChangeApprenticeshipSteps
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly TestContext _context;
        private ChangeApprenticeshipCommand _request = null!;
        private CommitmentStatement _commitmentStatement;
        private long _newApprenticeshipId;

        public ChangeApprenticeshipSteps(TestContext context)
        {
            _context = context;
            _commitmentStatement = _fixture.Create<CommitmentStatement>();
            _newApprenticeshipId = _fixture.Create<long>();
        }

        [Given("we have an existing apprenticeship")]
        public async Task GivenWeHaveAnExistingApprenticeship()
        {
            var apprentice = _fixture.Create<Apprentice>();
            apprentice.AddApprenticeship(_commitmentStatement);

            _context.DbContext.Apprentices.Add(apprentice);
            await _context.DbContext.SaveChangesAsync();
        }

        [Given("we do not have an existing apprenticeship")]
        public void GivenWeDoNotHaveAnExistingApprenticeship()
        {
        }

        [Given("we have an update apprenticeship request")]
        public void GivenWeHaveAnUpdateApprenticeshipRequest()
        {
            var start = _fixture.Create<DateTime>();
            _request = _fixture.Build<ChangeApprenticeshipCommand>()
                .Without(x=>x.ContinuationOfApprenticeshipId)
                .With(x => x.ApprenticeshipId, _commitmentStatement.CommitmentsApprenticeshipId)
                .With(x => x.Email, (MailAddress email) => email.ToString())
                .With(x => x.PlannedStartDate, start)
                .With(x => x.PlannedEndDate, (long days) => start.AddDays(days))
                .Create();
        }

        [Given("we have a update apprenticeship continuation request")]
        public void GivenWeHaveANewApprenticeshipRequest()
        {
            var start = _fixture.Create<DateTime>();
            _request = _fixture.Build<ChangeApprenticeshipCommand>()
                .With(x => x.ContinuationOfApprenticeshipId, _commitmentStatement.CommitmentsApprenticeshipId)
                .With(x => x.ApprenticeshipId, _newApprenticeshipId)
                .With(x => x.Email, (MailAddress email) => email.ToString())
                .With(x => x.PlannedStartDate, start)
                .With(x => x.PlannedEndDate, (long days) => start.AddDays(days))
                .Create();
        }

        [When("the update is posted")]
        public async Task WhenTheUpdateIsPosted()
        {
            await _context.Api.Post("apprenticeships/change", _request);
        }

        [Then("the result should return accepted")]
        public void ThenTheResultShouldReturnAccepted()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        }

        [Then("the result should return Not Found")]
        public void ThenTheResultShouldReturnNotFound()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Then("the new commitment statement exists in database")]
        public void ThenTheCommitmentStatementExistsInDatabase()
        {
            var cs = _context.DbContext.CommitmentStatements.ToList();

            _context.DbContext.CommitmentStatements.Should().ContainEquivalentOf(new
            {
                CommitmentsApprovedOn = _request.ApprovedOn,
                Details = new
                {
                    _request.EmployerAccountLegalEntityId,
                    _request.EmployerName,
                    _request.TrainingProviderId,
                    _request.TrainingProviderName,
                    Course = new
                    {
                        Name = _request.CourseName,
                        Level = _request.CourseLevel,
                        Option = _request.CourseOption,
                        _request.PlannedStartDate,
                        _request.PlannedEndDate,
                    },
                },
                TrainingProviderCorrect = (bool?)null,
                EmployerCorrect = (bool?)null,
                RolesAndResponsibilitiesCorrect = (bool?)null,
                ApprenticeshipDetailsCorrect = (bool?)null,
                HowApprenticeshipDeliveredCorrect = (bool?)null,
                ApprenticeshipConfirmed = (bool?)null,
            });
        }

        [Then("the new commitment statement has same commitments apprenticeship Id")]
        public void ThenTheCommitmentStatementHasSameCommitmentsApprenticeshipId()
        {
            var cs = _context.DbContext.CommitmentStatements.ToList();

            _context.DbContext.CommitmentStatements.Should().ContainEquivalentOf(new
            {
                CommitmentsApprovedOn = _request.ApprovedOn,
                CommitmentsApprenticeshipId = _commitmentStatement.CommitmentsApprenticeshipId
            });
        }

        [Then(@"the new commitment statement has a new commitments apprenticeship Id")]
        public void ThenTheNewCommitmentStatementHasANewCommitmentsApprenticeshipId()
        {
            var cs = _context.DbContext.CommitmentStatements.ToList();

            _context.DbContext.CommitmentStatements.Should().ContainEquivalentOf(new
            {
                CommitmentsApprovedOn = _request.ApprovedOn,
                CommitmentsApprenticeshipId = _newApprenticeshipId
            });
        }

        [Then("there should be no commitment statements in the database")]
        public void ThenThereShouldBeNoCommitmentStatementsInTheDatabase()
        {
            _context.DbContext.CommitmentStatements.Should().BeEmpty();
        }
    }
}