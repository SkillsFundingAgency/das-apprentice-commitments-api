﻿using AutoFixture;
using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Api.Controllers;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "HowApprenticeshipWillBeDelivered")]
    class HowApprenticeshipWillBeDeliveredSteps
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly TestContext _context;
        private readonly Apprentice _apprentice;
        private readonly CommitmentStatement _commitmentStatement;
        private bool? HowApprenticeshipDeliveredCorrect { get; set; }

        public HowApprenticeshipWillBeDeliveredSteps(TestContext context)
        {
            _context = context;
            _apprentice = _fixture.Create<Apprentice>();
            _commitmentStatement = _fixture.Create<CommitmentStatement>();
            _apprentice.AddApprenticeship(_commitmentStatement);
        }

        [Given(@"we have an apprenticeship waiting to be confirmed")]
        public async Task GivenWeHaveAnApprenticeshipWaitingToBeConfirmed()
        {
            _context.DbContext.Apprentices.Add(_apprentice);
            await _context.DbContext.SaveChangesAsync();
        }

        [Given("a HowApprenticeshipWillBeDeliveredRequest stating the HowMyApprenticeshipWillBeDelivered is correct")]
        public void GivenAHowApprenticeshipWillBeDeliveredRequestStatingTheHowMyApprenticeshipWillBeDeliveredIsCorrect()
        {
            HowApprenticeshipDeliveredCorrect = true;
        }

        [Given("a HowApprenticeshipWillBeDeliveredRequest stating the HowApprenticeshipWillBeDeliveredRequest is incorrect")]
        public void GivenAHowApprenticeshipWillBeDeliveredRequestStatingTheHowMyApprenticeshipWillBeDeliveredIsIncorrect()
        {
            HowApprenticeshipDeliveredCorrect = false;
        }

        [Given("we have an apprenticeship that has previously had HowMyApprenticeshipWillBeDelivered positively confirmed")]
        public async Task GivenWeHaveAnApprenticeshipThatHasPreviouslyHadHowMyApprenticeshipWillBeDeliveredPositivelyConfirmed()
        {
            _commitmentStatement.Confirm(new Confirmations { HowApprenticeshipDeliveredCorrect = true }, DateTimeOffset.Now);
            await GivenWeHaveAnApprenticeshipWaitingToBeConfirmed();
        }

        [When("we send the confirmation")]
        public async Task WhenWeSendTheConfirmation()
        {

            await _context.Api.Post(
                $"apprentices/{_apprentice.Id}/apprenticeships/{_commitmentStatement.ApprenticeshipId}/revisions/{_commitmentStatement.Id}/howapprenticeshipwillbedeliveredconfirmation",
                new ConfirmHowApprenticeshipWillBeDeliveredRequest
                {
                    HowApprenticeshipDeliveredCorrect = (bool)HowApprenticeshipDeliveredCorrect,
                });
        }

        [Then("the response is OK")]
        public void ThenTheResponseIsOK()
        {
            _context.Api.Response.EnsureSuccessStatusCode();
        }

        [Then("the apprenticeship record is updated")]
        public void ThenTheApprenticeshipRecordIsUpdated()
        {
            _context.DbContext.CommitmentStatements.Should().ContainEquivalentOf(new
            {
                _commitmentStatement.ApprenticeshipId,
                HowApprenticeshipDeliveredCorrect
            });
        }

        [Then(@"the response is BadRequest")]
        public void ThenTheResponseIsBadRequest()
        {
            _context.Api.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Then("the apprenticeship record remains unchanged")]
        public void ThenTheApprenticeshipRecordRemainsUnchanged()
        {
            _context.DbContext.CommitmentStatements
                .Should().ContainEquivalentOf(_commitmentStatement);
        }
    }
}
