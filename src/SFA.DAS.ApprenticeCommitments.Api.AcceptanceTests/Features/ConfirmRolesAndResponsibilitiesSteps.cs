﻿using AutoFixture;
using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Api.Controllers;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "ConfirmRolesAndResponsibilities")]
    public sealed class ConfirmRolesAndResponsibilitiesSteps
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly TestContext _context;
        private readonly Apprentice _apprentice;
        private readonly Apprenticeship _apprenticeship;
        private bool? RolesAndResponsibilitiesCorrect { get; set; }        

        public ConfirmRolesAndResponsibilitiesSteps(TestContext context)
        {
            _context = context;

            _apprentice = _fixture.Create<Apprentice>();
            _apprenticeship = _fixture.Create<Apprenticeship>();
            _apprentice.AddApprenticeship(_apprenticeship);
        }

        [Given("we have an apprenticeship waiting to be confirmed")]
        public async Task GivenWeHaveAnApprenticeshipWaitingToBeConfirmed()
        {
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();
        }

        [Given("we have an apprenticeship that has previously had its roles and responsibilities confirmed")]
        public async Task GivenWeHaveAnApprenticeshipThatHasPreviouslyHadItsRolesAndResponsibilitiesConfirmed()
        {
            _apprenticeship.ConfirmRolesAndResponsibilities(true);
            await GivenWeHaveAnApprenticeshipWaitingToBeConfirmed();
        }

        [Given("a ConfirmRolesAndResponsibilitiesRequest stating the roles and responsibilities are correct")]
        public void GivenAConfirmRolesAndResponsibilitiesRequestStatingTheRolesAndResponsibilitiesAreCorrect()
        {
            RolesAndResponsibilitiesCorrect = true;
        }

        [Given("a ConfirmRolesAndResponsibilitiesRequest stating the roles and responsibilities are incorrect")]
        public void GivenAConfirmRolesAndResponsibilitiesRequestStatingTheRolesAndResponsibilitiesAreIncorrect()
        {
            RolesAndResponsibilitiesCorrect = false;
        }

        [When("we send the confirmation")]
        public async Task WhenWeSendTheConfirmation()
        {
            var command = new ConfirmRolesAndResponsibilitiesRequest
            {
                RolesAndResponsibilitiesCorrect = (bool)RolesAndResponsibilitiesCorrect,
            };

            await _context.Api.Post(
                $"apprentices/{_apprentice.Id}/apprenticeships/{_apprenticeship.Id}/RolesAndResponsibilitiesConfirmation",
                command);
        }

        [Then("the response is OK")]
        public void ThenTheResponseIsOK()
        {
            _context.Api.Response.EnsureSuccessStatusCode();
        }

        [Then("the response is BadRequest")]
        public void ThenTheResponseIsBadRequest()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Then("the apprenticeship record is updated")]
        public void ThenTheApprenticeshipRecordIsUpdated()
        {
            _context.DbContext.Apprenticeships.Should().ContainEquivalentOf(new
            {
                _apprenticeship.Id,
                RolesAndResponsibilitiesCorrect,
            });
        }

        [Then("the apprenticeship record remains unchanged")]
        public void ThenTheApprenticeshipRecordRemainsUnchanged()
        {
            _context.DbContext.Apprenticeships
                .Should().ContainEquivalentOf(_apprenticeship,
                    compare => compare.Excluding(x => x.Apprentice));
        }
    }
}
