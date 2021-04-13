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
    [Scope(Feature = "HowApprenticeshipWillBeDelivered")]
    class HowApprenticeshipWillBeDeliveredSteps
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly TestContext _context;
        private readonly Apprentice _apprentice;
        private readonly Apprenticeship _apprenticeship;
        private bool? HowApprenticeshipDeliveredCorrect { get; set; }
        private string endpoint;
        private object command;

        public HowApprenticeshipWillBeDeliveredSteps(TestContext context)
        {
            _context = context;
            _apprentice = _fixture.Create<Apprentice>();
            _apprenticeship = _fixture.Create<Apprenticeship>();
            _apprentice.AddApprenticeship(_apprenticeship);
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
            endpoint = "howapprenticeshipwillbedeliveredconfirmation";
            HowApprenticeshipDeliveredCorrect = true;
            command = new HowApprenticeshipWillBeDeliveredRequest
            {
                HowApprenticeshipDeliveredCorrect = true,
            };
        }

        [Given("a HowApprenticeshipWillBeDeliveredRequest stating the HowApprenticeshipWillBeDeliveredRequest is incorrect")]
        public void GivenAHowApprenticeshipWillBeDeliveredRequestStatingTheHowMyApprenticeshipWillBeDeliveredIsIncorrect()
        {
            endpoint = "howapprenticeshipwillbedeliveredconfirmation";
            HowApprenticeshipDeliveredCorrect = false;
            command = new HowApprenticeshipWillBeDeliveredRequest
            {
                HowApprenticeshipDeliveredCorrect = false,
            };
        }

        [Given("we have an apprenticeship that has previously had HowMyApprenticeshipWillBeDelivered positively confirmed")]
        public async Task GivenWeHaveAnApprenticeshipThatHasPreviouslyHadHowMyApprenticeshipWillBeDeliveredPositivelyConfirmed()
        {
            _apprenticeship.ConfirmHowApprenticeshipWillBeDelivered(true);
            await GivenWeHaveAnApprenticeshipWaitingToBeConfirmed();
        }

        [Given("a HowApprenticeshipWillBeDeliveredRequest stating the HowMyApprenticeshipWillBeDelivered page is not understood")]
        public void GivenAHowApprenticeshipWillBeDeliveredRequestStatingTheHowMyApprenticeshipWillBeDeliveredPageIsNotUnderstood()
        {
            endpoint = "howapprenticeshipwillbedeliveredconfirmation";
            HowApprenticeshipDeliveredCorrect = false;
            command = new HowApprenticeshipWillBeDeliveredRequest
            {
                HowApprenticeshipDeliveredCorrect = false,
            };
        }

        [When("we send the confirmation")]
        public async Task WhenWeSendTheConfirmation()
        {
            await _context.Api.Post(
                $"apprentices/{_apprentice.Id}/apprenticeships/{_apprenticeship.Id}/{endpoint}",
                command);
        }

        [Then("the response is OK")]
        public void ThenTheResponseIsOK()
        {
            _context.Api.Response.EnsureSuccessStatusCode();
        }

        [Then("the apprenticeship record is updated")]
        public void ThenTheApprenticeshipRecordIsUpdated()
        {
            _context.DbContext.Apprenticeships.Should().ContainEquivalentOf(new
            {
                _apprenticeship.Id,
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
            _context.DbContext.Apprenticeships
                .Should().ContainEquivalentOf(_apprenticeship,
                    compare => compare.Excluding(x => x.Apprentice));
        }
    }
}
