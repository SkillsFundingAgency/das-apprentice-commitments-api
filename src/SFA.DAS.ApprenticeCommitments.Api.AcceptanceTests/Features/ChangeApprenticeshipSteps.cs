using AutoFixture;
using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Api.Extensions;
using TechTalk.SpecFlow;
using NServiceBus.Testing;
using SFA.DAS.ApprenticeCommitments.Messages.Events;

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
        private long _commitmentsApprenticeshipId;

        public ChangeApprenticeshipSteps(TestContext context)
        {
            _context = context;
            _commitmentsApprenticeshipId = _fixture.Create<long>();
            _commitmentStatement = _fixture.Create<CommitmentStatement>();
            _commitmentStatement.SetProperty(p => p.CommitmentsApprenticeshipId, _commitmentsApprenticeshipId);
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

        [Given("we do not have an existing apprenticeship, confirmed or unconfirmed")]
        public void GivenWeDoNotHaveAnExistingApprenticeshipConfirmedOrUnconfirmed()
        {
        }

        [Given("we do not have an existing apprenticeship")]
        public void GivenWeDoNotHaveAnExistingApprenticeship()
        {
        }

        [Given(@"we do have a verified registration")]
        public async Task GivenWeDoHaveAVerifiedRegistration()
        {
            var registration = _fixture.Create<Registration>();
            registration.SetProperty(x => x.CommitmentsApprenticeshipId, _commitmentsApprenticeshipId);
            registration.SetProperty(x => x.UserIdentityId, Guid.NewGuid());

            _context.DbContext.Registrations.Add(registration);
            await _context.DbContext.SaveChangesAsync();
        }

        [Given(@"we do have an unconfirmed registration")]
        public async Task GivenWeDoHaveAnUnconfirmedRegistration()
        {
            var registration = _fixture.Create<Registration>();
            registration.SetProperty(x => x.CommitmentsApprenticeshipId, _commitmentsApprenticeshipId);

            _context.DbContext.Registrations.Add(registration);
            await _context.DbContext.SaveChangesAsync();
        }

        [Given("we have an update apprenticeship request")]
        public void GivenWeHaveAnUpdateApprenticeshipRequest()
        {
            var start = _fixture.Create<DateTime>();
            _request = _fixture.Build<ChangeApprenticeshipCommand>()
                .Without(x => x.CommitmentsContinuedApprenticeshipId)
                .With(x => x.CommitmentsApprenticeshipId, _commitmentsApprenticeshipId)
                .With(x => x.PlannedStartDate, start)
                .With(x => x.PlannedEndDate, (long days) => start.AddDays(days))
                .Create();
        }

        [Given("we have an update apprenticeship request with no material change")]
        public void GivenWeHaveAnInconsequenticalUpdateApprenticeshipRequest()
        {
            _request = _fixture.Build<ChangeApprenticeshipCommand>()
                .Without(x => x.CommitmentsContinuedApprenticeshipId)
                .With(x => x.CommitmentsApprenticeshipId, _commitmentsApprenticeshipId)
                .With(x => x.EmployerAccountLegalEntityId, _commitmentStatement.Details.EmployerAccountLegalEntityId)
                .With(x => x.EmployerName, _commitmentStatement.Details.EmployerName)
                .With(x => x.TrainingProviderId, _commitmentStatement.Details.TrainingProviderId)
                .With(x => x.TrainingProviderName, _commitmentStatement.Details.TrainingProviderName)
                .With(x => x.CourseName, _commitmentStatement.Details.Course.Name)
                .With(x => x.CourseLevel, _commitmentStatement.Details.Course.Level)
                .With(x => x.CourseOption, _commitmentStatement.Details.Course.Option)
                .With(x => x.PlannedStartDate, _commitmentStatement.Details.Course.PlannedStartDate)
                .With(x => x.PlannedEndDate, _commitmentStatement.Details.Course.PlannedEndDate)
                .Create();
        }

        [Given("we have a update apprenticeship continuation request")]
        public void GivenWeHaveANewApprenticeshipRequest()
        {
            var start = _fixture.Create<DateTime>();
            _request = _fixture.Build<ChangeApprenticeshipCommand>()
                .With(x => x.CommitmentsContinuedApprenticeshipId, _commitmentsApprenticeshipId)
                .With(x => x.CommitmentsApprenticeshipId, _newApprenticeshipId)
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
                CommitmentsApprovedOn = _request.CommitmentsApprovedOn,
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
                ApprenticeshipConfirmed = false,
            });
        }

        [Then(@"we have updated the apprenticeship details for the unconfirmed registration")]
        public void ThenWeHaveUpdatedTheApprenticeshipDetailsForTheUnconfirmedRegistration()
        {
            _context.DbContext.Registrations.Should().ContainEquivalentOf(new
            {
                Apprenticeship = new
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
                }
            });
        }

        [Then("the new commitment statement has same commitments apprenticeship Id")]
        public void ThenTheCommitmentStatementHasSameCommitmentsApprenticeshipId()
        {
            _context.DbContext.CommitmentStatements.Should().ContainEquivalentOf(new
            {
                CommitmentsApprovedOn = _request.CommitmentsApprovedOn,
                CommitmentsApprenticeshipId = _commitmentStatement.CommitmentsApprenticeshipId
            });
        }

        [Then(@"the new commitment statement has a new commitments apprenticeship Id")]
        public void ThenTheNewCommitmentStatementHasANewCommitmentsApprenticeshipId()
        {
            _context.DbContext.CommitmentStatements.Should().ContainEquivalentOf(new
            {
                CommitmentsApprovedOn = _request.CommitmentsApprovedOn,
                CommitmentsApprenticeshipId = _request.CommitmentsApprenticeshipId
            });
        }

        [Then(@"registration commitments apprenticeship are updated correctly")]
        public void ThenRegistrationCommitmentsApprenticeshipAreUpdatedCorrectly()
        {
            _context.DbContext.Registrations.Should().ContainEquivalentOf(new
            {
                CommitmentsApprovedOn = _request.CommitmentsApprovedOn,
                CommitmentsApprenticeshipId = _request.CommitmentsApprenticeshipId
            });
        }

        [Then("there should only be the original commitment statement in the database")]
        public void ThenThereShouldOnlyBeTheOriginalCommitmentStatementInTheDatabase()
        {
            _context.DbContext.CommitmentStatements.Should().HaveCount(1);
        }

        [Then("there should be no commitment statements in the database")]
        public void ThenThereShouldBeNoCommitmentStatementsInTheDatabase()
        {
            _context.DbContext.CommitmentStatements.Should().BeEmpty();
        }

        [Then(@"a domain exception is thrown")]
        public async Task ThenADomainExceptionIsThrown()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeObject<List<ErrorItem>>(content);
            errors.Count.Should().Be(1);
            errors[0].PropertyName.Should().BeNull();
            errors[0].ErrorMessage.Should().NotBeNull();
        }

        [Then("the response is bad request")]
        public void ThenTheResponseIsOK()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Then("the Confirmation Commenced event is published")]
        public void ThenTheConfirmationStartedEventIsPublished()
        {
            var latest = _context.DbContext.CommitmentStatements.OrderBy(x => x.Id).Last();

            _context.Messages.PublishedMessages.Should().ContainEquivalentOf(new
            {
                Message = new ApprenticeshipConfirmationCommencedEvent
                {
                    ApprenticeshipId = _commitmentStatement.ApprenticeshipId,
                    ConfirmationOverdueOn = latest.ConfirmBefore,
                    CommitmentsApprovedOn = _request.CommitmentsApprovedOn,
                    CommitmentsApprenticeshipId = _request.CommitmentsApprenticeshipId,
                }
            });
        }

        [Then("no Confirmation Commenced event is published")]
        public void ThenNoConfirmationCommencedEventIsPublished()
        {
            _context.Messages.PublishedMessages
                .Select(x => x.Message is ApprenticeshipConfirmationCommencedEvent)
                .Should().BeEmpty();
        }
    }
}