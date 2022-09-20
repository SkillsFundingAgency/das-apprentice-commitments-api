using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Application.Queries.ApprovalsRegistrationQuery;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.DTOs;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "GetApprovalsRegistration")]
    public class GetRegistrationSteps
    {
        private readonly TestContext _context;
        private Fixture _fixture;
        private Registration _registration;

        public GetRegistrationSteps(TestContext context)
        {
            _fixture = new Fixture();
            _registration = _fixture.Create<Registration>();
            _context = context;
        }

        [Given("there is no registration")]
        public static void GivenThereIsNoRegistration()
        {
        }

        [Given(@"there is a registration with an (.*) assigned to it")]
        public Task GivenThereIsARegistrationWithAnAssignedToIt(string apprenticeId)
        {
            _registration.SetProperty(x => x.ApprenticeId, apprenticeId.ToGuid());
            _context.DbContext.Registrations.Add(_registration);
            return _context.DbContext.SaveChangesAsync();
        }


        [When("we try to retrieve the registration using approvals identity")]
        public Task WhenWeTryToRetrieveTheRegistration()
        {
            return _context.Api.Get($"approvals/{_registration.CommitmentsApprenticeshipId}/registration");
        }

        [Given("there is an empty commitments apprenticeship id")]
        public void GivenThereIsAnEmptyId()
        {
            _fixture.Inject(0L);
            _registration = _fixture.Build<Registration>().Create();
        }

        [When("we try to retrieve the registration using a bad request format")]
        public Task WhenWeTryToRetrieveTheRegistrationUsingABadRequestFormat()
        {
            return _context.Api.Get($"approvals/1234-1234/registration");
        }

        [Then("the result should return ok")]
        public void ThenTheResultShouldReturnOk()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the response should match the registration in the database with (.*)")]
        public async Task ThenTheResponseShouldMatchTheRegistrationInTheDatabaseWith(string apprenticeId)
        {


            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            content.Should().NotBeNull();
            var response = JsonConvert.DeserializeObject<RegistrationDto>(content);
            response.Email.Should().Be(_registration.Email.ToString());
            response.RegistrationId.Should().Be(_registration.RegistrationId);
            response.ApprenticeId.Should().Be(apprenticeId.ToGuid());
        }

        [Then("the result should return not found")]
        public void ThenTheResultShouldReturnNotFound()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Then("the result should return bad request")]
        public void ThenTheResultShouldReturnBadRequest()
        {
            _context.Api.Response.Should().Be400BadRequest();
        }

        [Then("the error must be say approvals apprenticeship identity must be valid")]
        public async Task ThenTheErrorMustBeSayRegistrationMustBeValid()
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            content.Should().NotBeNull();
            var response = JsonConvert.DeserializeObject<ValidationProblemDetails>(content);
            response.Errors.Should().ContainKey("CommitmentsApprenticeshipId")
                .WhoseValue.Should().Contain("The approvals apprenticeship identity must be valid");
        }
    }



}