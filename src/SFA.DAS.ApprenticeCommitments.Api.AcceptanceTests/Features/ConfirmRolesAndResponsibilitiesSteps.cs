using AutoFixture;
using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
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
        private readonly Revision _revision;

        private RolesAndResponsibilitiesConfirmations _rolesAndResponsibilitiesConfirmations; 

        public ConfirmRolesAndResponsibilitiesSteps(TestContext context)
        {
            _context = context;

            _apprentice = _fixture.Create<Apprentice>();
            _revision = _fixture.Create<Revision>();
            _apprentice.AddApprenticeship(_revision);
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
            _revision.Confirm(new Confirmations
            {
                RolesAndResponsibilitiesConfirmations = RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed &
                                                        RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed &
                                                        RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed,
            }, _fixture.Create<DateTimeOffset>());
            await GivenWeHaveAnApprenticeshipWaitingToBeConfirmed();
        }

        [Given(@"a confirmation stating the roles and responsibilities for have been read for (.*) is set")]
        public void GivenAConfirmationStatingTheRolesAndResponsibilitiesForHaveBeenReadForIsSet(RolesAndResponsibilitiesConfirmations hasRead)
        {
            _rolesAndResponsibilitiesConfirmations = hasRead;
        }

        [Given(@"we have an apprenticeship with ApprenticeRolesAndResponsibilitiesConfirmed")]
        public async Task GivenWeHaveAnApprenticeshipWithApprenticeRolesAndResponsibilitiesConfirmed()
        {
            _revision.Confirm(new Confirmations
            {
                RolesAndResponsibilitiesConfirmations = RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed
            }, _fixture.Create<DateTimeOffset>());

            await GivenWeHaveAnApprenticeshipWaitingToBeConfirmed();
        }

        [Given(
            @"we have an apprenticeship with ApprenticeRolesAndResponsibilitiesConfirmed and EmployerRolesAndResponsibilitiesConfirmed")]
        public async Task
            GivenWeHaveAnApprenticeshipWithApprenticeRolesAndResponsibilitiesConfirmedAndEmployerRolesAndResponsibilitiesConfirmed()
        {
            _revision.Confirm(new Confirmations
            {
                RolesAndResponsibilitiesConfirmations =
                    RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed &
                    RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed
            }, _fixture.Create<DateTimeOffset>());

            await GivenWeHaveAnApprenticeshipWaitingToBeConfirmed();
        }

        [When("we send the confirmation")]
        public async Task WhenWeSendTheConfirmation()
        {
            var command = new Confirmations
            {
                RolesAndResponsibilitiesConfirmations = _rolesAndResponsibilitiesConfirmations,
            };

            await _context.Api.Patch(
                $"apprentices/{_apprentice.Id}/apprenticeships/{_revision.ApprenticeshipId}/revisions/{_revision.Id}/confirmations",
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
            _context.DbContext.Revisions.Should().ContainEquivalentOf(new
            {
                _revision.ApprenticeshipId,
                RolesAndResponsibilitiesConfirmations = _rolesAndResponsibilitiesConfirmations,
            });
        }

        [Then(@"the apprenticeship record now contains ApprenticeRolesAndResponsibilitiesConfirmed and (.*)")]
        public void ThenTheApprenticeshipRecordNowContainsApprenticeRolesAndResponsibilitiesConfirmedAnd(RolesAndResponsibilitiesConfirmations p0)
        {
            _context.DbContext.Revisions.Should().ContainEquivalentOf(new
            {
                _revision.ApprenticeshipId,
                RolesAndResponsibilitiesConfirmations = RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed & _rolesAndResponsibilitiesConfirmations,
            });
        }

        [Then(@"the apprenticeship record shows the roles and responsibilities is fully confirmed")]
        public void ThenTheApprenticeshipRecordNowShowsTheRolesAndResponsibilitiesIsFullyConfirmed()
        {
            _context.DbContext.Revisions.Should().ContainEquivalentOf(new
            {
                _revision.ApprenticeshipId,
                RolesAndResponsibilitiesConfirmations =
                    RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed &
                    RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed &
                    RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed,
            });
        }


        [Then("the apprenticeship record remains unchanged")]
        public void ThenTheApprenticeshipRecordRemainsUnchanged()
        {
            _context.DbContext.Revisions
                .Should().ContainEquivalentOf(_revision);
        }
    }
}
