using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Api.Extensions;
using SFA.DAS.ApprenticeCommitments.Application.Commands.VerifyRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "VerifyRegistration")]
    public class VerifyRegistrationSteps
    {
        private readonly TestContext _context;
        private VerifyRegistrationCommand _command;
        private Fixture _f;
        private Registration _registration;
        private Guid _missingApprenticeId;
        private Guid _apprenticeId;

        public VerifyRegistrationSteps(TestContext context)
        {
            _context = context;
            _f = new Fixture();
            _missingApprenticeId = _f.Create<Guid>();
        }

        [Given(@"we have an existing registration")]
        public void GivenWeHaveAnExistingRegistration()
        {
            _registration = _f.Create<Registration>();
            _context.DbContext.Registrations.Add(_registration);
            _context.DbContext.SaveChanges();
        }

        [Given(@"the request matches registration details")]
        public void GivenTheRequestMatchesRegistrationDetails()
        {
            _command = _f.Build<VerifyRegistrationCommand>()
                .With(p => p.Email, _registration.Email.ToString)
                .With(p => p.ApprenticeId, _registration.RegistrationId)
                .With(p=>p.DateOfBirth, _registration.DateOfBirth)
                .Create();
        }

        [Given(@"the request email does not match")]
        public void GivenTheRequestEmailDoesNotMatch()
        {
            _command = _f.Build<VerifyRegistrationCommand>()
                .With(p => p.Email, "another@email.com")
                .With(p => p.ApprenticeId, _registration.RegistrationId)
                .With(p => p.DateOfBirth, _registration.DateOfBirth)
                .Create();
        }

        [Given(@"the request date of birth does not match")]
        public void GivenTheRequestDateOfBirthDoesNotMatch()
        {
            _command = _f.Build<VerifyRegistrationCommand>()
                .With(p => p.Email, _registration.Email.ToString)
                .With(p => p.ApprenticeId, _registration.RegistrationId)
                .With(p => p.DateOfBirth, _registration.DateOfBirth.AddDays(90))
                .Create();
        }

        [Given(@"we have an existing already verified registration")]
        public void GivenWeHaveAnExistingAlreadyVerifiedRegistration()
        {
            _registration = _f.Create<Registration>();
            _registration.ConvertToApprentice(
                "", "", _registration.Email, DateTime.Now, Guid.NewGuid());

            _context.DbContext.Registrations.Add(_registration);
            _context.DbContext.SaveChanges();
        }

        [Given(@"the verify registration request is invalid")]
        public void GivenTheVerifyRegistrationRequestIsInvalid()
        {
            _command = new VerifyRegistrationCommand();
        }

        [Given(@"we do NOT have an existing registration")]
        public void GivenWeDoNOTHaveAnExistingRegistration()
        {
        }

        [Given(@"a valid registration request is submitted")]
        public void GivenAValidRegistrationRequestIsSubmitted()
        {
            _command = _f.Build<VerifyRegistrationCommand>()
                .With(p => p.Email, "another@email.com")
                .With(p => p.ApprenticeId, _missingApprenticeId)
                .Create();
        }

        [Given(@"the verify registration request is valid except email address")]
        public void GivenTheVerifyRegistrationRequestIsValidExceptEmailAddress()
        {
            _command = _f.Build<VerifyRegistrationCommand>()
                .With(p => p.Email, "missingemail.com")
                .Create();
        }

        [When(@"we verify that registration")]
        public async Task WhenWeVerifyThatRegistration()
        {
            await _context.Api.Post("registrations", _command);
        }

        [Then(@"the apprentice record is created")]
        public void ThenTheApprenticeRecordIsCreated()
        {
            var apprentice = _context.DbContext.Apprentices.FirstOrDefault(x => x.Id == _command.ApprenticeId);
            apprentice.Should().NotBeNull();
            apprentice.FirstName.Should().Be(_command.FirstName);
            apprentice.LastName.Should().Be(_command.LastName);
            apprentice.Email.Should().Be(_registration.Email);
            apprentice.DateOfBirth.Should().Be(_command.DateOfBirth);
            apprentice.Id.Should().Be(_command.ApprenticeId);
            _apprenticeId = apprentice.Id;
        }

        [Then("an apprenticeship record is not yet created")]
        public void ThenAnApprenticeshipRecordIsNotYetCreated()
        {
            var apprentice = _context.DbContext
                .Apprentices.Include(x => x.Apprenticeships).ThenInclude(x => x.CommitmentStatements)
                .Should().Contain(x => x.Id == _command.ApprenticeId)
                .Which.Apprenticeships.Should().BeEmpty();
        }

        [Then(@"an apprenticeship record is created")]
        public void ThenAnApprenticeshipRecordIsCreated()
        {
            var apprentice = _context.DbContext
                .Apprentices.Include(x => x.Apprenticeships).ThenInclude(x => x.CommitmentStatements)
                .FirstOrDefault(x => x.Id == _command.ApprenticeId);

            apprentice.Apprenticeships.SelectMany(a => a.CommitmentStatements)
                .Should().ContainEquivalentOf(new
            {
                CommitmentsApprenticeshipId = _registration.CommitmentsApprenticeshipId,
                Details = new
                {
                    _registration.Apprenticeship.EmployerName,
                    _registration.Apprenticeship.EmployerAccountLegalEntityId,
                    _registration.Apprenticeship.TrainingProviderId,
                    _registration.Apprenticeship.TrainingProviderName,
                    Course = new
                    {
                        _registration.Apprenticeship.Course.Name,
                        _registration.Apprenticeship.Course.Level,
                        _registration.Apprenticeship.Course.Option,
                        _registration.Apprenticeship.Course.PlannedStartDate,
                        _registration.Apprenticeship.Course.PlannedEndDate,
                    }
                },
            });
        }

        [Then("the Confirmation Commenced event is published")]
        public void ThenTheConfirmationStartedEventIsPublished()
        {
            var latest = _context.DbContext.CommitmentStatements.Single();

            _context.Messages.PublishedMessages.Should().ContainEquivalentOf(new
            {
                Message = new ApprenticeshipConfirmationCommencedEvent
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

        [Then(@"the registration has been marked as completed")]
        public void ThenTheRegistrationHasBeenMarkedAsCompleted()
        {
            var registration = _context.DbContext.Registrations.FirstOrDefault(x => x.RegistrationId == _registration.RegistrationId);
            registration.UserIdentityId.Should().Be(_command.UserIdentityId);
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

        [Then(@"a bad request is returned")]
        public void ThenABadRequestIsReturned()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Then(@"a email domain error is returned")]
        public async Task ThenAEmailDomainErrorIsReturned()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeObject<List<ErrorItem>>(content);
            errors.Count.Should().Be(1);
            errors[0].PropertyName.Should().Be("PersonalDetails");
            errors[0].ErrorMessage.Should().Be("Sorry, your identity has not been verified, please check your details");
        }

        [Then(@"an identity mismatch domain error is returned")]
        public async Task ThenAnIdentityMismatchDomainErrorIsReturned()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeObject<List<ErrorItem>>(content);
            errors.Count.Should().Be(1);
            errors[0].PropertyName.Should().Be("PersonalDetails");
        }

        [Then(@"an 'already verified' domain error is returned")]
        public async Task ThenAnAlreadyVerifiedDomainErrorIsReturned()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeObject<List<ErrorItem>>(content);
            errors.Should().ContainEquivalentOf(new ErrorItem
            {
                PropertyName = null,
                ErrorMessage = $"Registration {_registration.RegistrationId} is already verified",
            });
        }

        [Then(@"response contains the expected error messages")]
        public async Task ThenResponseContainsTheExpectedErrorMessages()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeObject<List<ErrorItem>>(content);

            errors.Should().ContainEquivalentOf(new { PropertyName = "ApprenticeId" });
            errors.Should().ContainEquivalentOf(new { PropertyName = "UserIdentityId" });
            errors.Should().ContainEquivalentOf(new { PropertyName = "FirstName" });
            errors.Should().ContainEquivalentOf(new { PropertyName = "LastName" });
            errors.Should().ContainEquivalentOf(new { PropertyName = "DateOfBirth" });
            errors.Should().ContainEquivalentOf(new { PropertyName = "Email" });
        }

        [Then(@"response contains the expected email error message")]
        public async Task ThenResponseContainsTheExpectedEmailErrorMessage()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeObject<List<ErrorItem>>(content);

            errors.Count(x => x.PropertyName == "Email").Should().Be(1);
        }

        [Then(@"response contains the not found error message")]
        public async Task ThenResponseContainsTheNotFoundErrorMessage()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeObject<List<ErrorItem>>(content);

            errors.Count().Should().Be(1);
            errors[0].ErrorMessage.Should().Be($"Registration for Apprentice {_missingApprenticeId} not found");
        }

        [Then("a record of the apprentice email address is kept")]
        public void ThenTheChangeHistoryIsRecorded()
        {
            var modified = _context.DbContext
                .Apprentices.Include(x => x.PreviousEmailAddresses)
                .Single(x => x.Id == _apprenticeId);

            modified.PreviousEmailAddresses.Should().ContainEquivalentOf(new
            {
                EmailAddress = new MailAddress(_command.Email),
            });
        }

        [Then("do not send a Change of Circumstance email to the user")]
        public void ThenDoNotSendAChangeOfCircumstanceEmailToTheUser()
        {
            _context.Messages.PublishedMessages
                .Should().NotContain(x => x.Message is ApprenticeshipChangedEvent);
        }
    }
}