using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateApprenticeshipFromRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Data.FuzzyMatching;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "CreateApprenticeshipFromRegistration")]
    public class CreateApprenticeshipFromRegistrationSteps
    {
        private readonly TestContext _context;
        private CreateApprenticeshipFromRegistrationCommand _command;
        private Fixture _f;
        private Registration _registration;
        private Guid _apprenticeId;
        private string _lastName;
        private DateTime _dateOfBirth;

        public CreateApprenticeshipFromRegistrationSteps(TestContext context)
        {
            _context = context;
            _f = new Fixture();
        }

        [Given(@"we have an existing registration")]
        public void GivenWeHaveAnExistingRegistration()
        {
            _f.Inject(DeliveryModel.PortableFlexiJob);
            _registration = _f.Create<Registration>();
            _context.DbContext.Registrations.Add(_registration);
            _context.DbContext.SaveChanges();
        }

        [Given("we have a matching account")]
        public void GivenWeHaveAnExistingAccount()
        {
            _apprenticeId = _f.Create<Guid>();
            _lastName = _registration.LastName;
            _dateOfBirth = _registration.DateOfBirth;
        }

        [Given("we have an account with a non-matching date of birth")]
        public void GivenWeHaveAnAccountWithANon_MatchingDateOfBirth()
        {
            _dateOfBirth = _registration.DateOfBirth.AddDays(90);
        }

        [Given("the request matches registration details")]
        [Given("the request is for the account")]
        public void GivenTheRequestMatchesRegistrationDetails()
        {
            _command = _f.Build<CreateApprenticeshipFromRegistrationCommand>()
                .With(p => p.RegistrationId, _registration.RegistrationId)
                .With(p => p.ApprenticeId, _apprenticeId)
                .With(p => p.LastName, _lastName)
                .With(p => p.DateOfBirth, _dateOfBirth)
                .Create();
        }

        [Given("the request is for a different account")]
        public void GivenTheRequestIsForADifferentAccount()
        {
            _command = _f.Build<CreateApprenticeshipFromRegistrationCommand>()
                .With(p => p.ApprenticeId, Guid.NewGuid())
                .With(p => p.RegistrationId, _registration.RegistrationId)
                .With(p => p.LastName, _registration.LastName)
                .With(p => p.DateOfBirth, _registration.DateOfBirth)
                .Create();
        }

        [Given(@"we have an existing already verified registration")]
        public void GivenWeHaveAnExistingAlreadyVerifiedRegistration()
        {
            GivenWeHaveAnExistingRegistration();
            GivenWeHaveAnExistingAccount();

            _registration.AssociateWithApprentice(_apprenticeId, _lastName, _dateOfBirth, FuzzyMatcher.AlwaysMatcher);
            _context.DbContext.SaveChanges();
        }

        [Given(@"the verify registration request is invalid")]
        public void GivenTheVerifyRegistrationRequestIsInvalid()
        {
            _command = null;
        }

        [Given(@"we do NOT have an existing registration")]
        public static void GivenWeDoNOTHaveAnExistingRegistration()
        {
        }

        [Given(@"a valid registration request is submitted")]
        public void GivenAValidRegistrationRequestIsSubmitted()
        {
            _command = _f.Create<CreateApprenticeshipFromRegistrationCommand>();
        }

        [When(@"we verify that registration")]
        public async Task WhenWeVerifyThatRegistration()
        {
            await _context.Api.Post("apprenticeships", _command);
        }

        [Then("an apprenticeship record is not yet created")]
        public void ThenAnApprenticeshipRecordIsNotYetCreated()
        {
            _context.DbContext.Apprenticeships.Should().BeEmpty();
        }

        [Then(@"an apprenticeship record is created")]
        public void ThenAnApprenticeshipRecordIsCreated()
        {
            var apprenticeships = _context.DbContext
                .Apprenticeships
                .Include(x => x.Revisions)
                .Where(x => x.ApprenticeId == _command.ApprenticeId);

            apprenticeships.SelectMany(a => a.Revisions)
                .Should().ContainEquivalentOf(new
                {
                    _registration.CommitmentsApprenticeshipId,
                    Details = new
                    {
                        _registration.Approval.EmployerName,
                        _registration.Approval.EmployerAccountLegalEntityId,
                        _registration.Approval.TrainingProviderId,
                        _registration.Approval.TrainingProviderName,
                        _registration.Approval.DeliveryModel,
                        Course = new
                        {
                            _registration.Approval.Course.Name,
                            _registration.Approval.Course.Level,
                            _registration.Approval.Course.Option,
                            _registration.Approval.Course.PlannedStartDate,
                            _registration.Approval.Course.PlannedEndDate,
                            _registration.Approval.Course.EmploymentEndDate,
                        }
                    },
                });
        }

        [Then("the Confirmation Commenced event is published")]
        public void ThenTheConfirmationStartedEventIsPublished()
        {
            var latest = _context.DbContext.Revisions.Single();

            _context.PublishedNServiceBusEvents.Should().ContainEquivalentOf(new
            {
                Event = new ApprenticeshipConfirmationCommencedEvent
                {
                    ApprenticeId = _registration.RegistrationId,
                    ApprenticeshipId = latest.ApprenticeshipId,
                    ConfirmationId = latest.Id,
                    ConfirmationOverdueOn = latest.ConfirmBefore,
                    CommitmentsApprovedOn = _registration.CommitmentsApprovedOn,
                    CommitmentsApprenticeshipId = _registration.CommitmentsApprenticeshipId,
                }
            });
        }

        [Then("the response is OK")]
        public void ThenTheResponseIsOK() => _context.Api.Response.Should().Be2XXSuccessful();

        [Then(@"the registration has been marked as completed")]
        public void ThenTheRegistrationHasBeenMarkedAsCompleted()
        {
            var registration = _context.DbContext.Registrations.FirstOrDefault(x => x.RegistrationId == _registration.RegistrationId);
            registration.ApprenticeId.Should().Be(_command.ApprenticeId);
        }

        [Then(@"the registration CreatedOn field is unchanged")]
        public void ThenTheRegistrationCreatedOnFieldIsUnchanged()
        {
            _context.DbContext.Registrations.Should().ContainEquivalentOf(new
            {
                _registration.RegistrationId,
                _registration.CreatedOn
            });
        }

        [Then(@"the apprenticeship email address confirmed event is published")]
        public void ThenTheApprenticeshipEmailAddressConfirmedEventIsPublished()
        {
            _context.PublishedNServiceBusEvents.Should().ContainEquivalentOf(new
            {
                Event = new ApprenticeshipEmailAddressConfirmedEvent
                {
                    ApprenticeId = _apprenticeId,
                    CommitmentsApprenticeshipId = _registration.CommitmentsApprenticeshipId,
                }
            });
        }

        [Then(@"a bad request is returned")]
        public void ThenABadRequestIsReturned()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Then(@"a email domain error is returned")]
        public async Task ThenAEmailDomainErrorIsReturned()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeObject<ValidationProblemDetails>(content);
            errors.Errors.Should().ContainKey("PersonalDetails")
                .WhoseValue.Should().Contain("Sorry, your identity has not been verified, please check your details");
        }

        [Then(@"an identity mismatch domain error is returned")]
        public async Task ThenAnIdentityMismatchDomainErrorIsReturned()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeObject<ValidationProblemDetails>(content);
            errors.Errors.Should().ContainKey("PersonalDetails")
                .WhoseValue.Should().Contain("Sorry, your identity has not been verified, please check your details");
        }

        [Then(@"an 'already verified' domain error is returned")]
        public async Task ThenAnAlreadyVerifiedDomainErrorIsReturned()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            content.Should().Contain($"Registration {_registration.RegistrationId} is already verified");
        }

        [Then("response contains the expected error messages")]
        public async Task ThenResponseContainsTheExpectedErrorMessages()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeObject<ValidationProblemDetails>(content);

            errors.Errors.Should().ContainKey("")
                .WhoseValue.Should().Contain("A non-empty request body is required.");
        }

        [Then(@"response contains the not found error message")]
        public async Task ThenResponseContainsTheNotFoundErrorMessage()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeObject<ProblemDetails>(content);
            errors.Should().BeEquivalentTo(new
            {
                Detail = $"Registration for Apprentice {_command.RegistrationId} not found",
            });
        }

        [Then("do not send a Change of Circumstance email to the user")]
        public void ThenDoNotSendAChangeOfCircumstanceEmailToTheUser()
        {
            _context.PublishedNServiceBusEvents.Should().NotContain(x => x.Event is ApprenticeshipChangedEvent);
        }
    }
}