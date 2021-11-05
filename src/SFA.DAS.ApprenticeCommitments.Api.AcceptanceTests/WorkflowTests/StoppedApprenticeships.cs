using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Commands.StoppedApprenticeshipCommand;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    internal class StoppedApprenticeships : ApiFixture
    {
        [Test, AutoData]
        public async Task Stopped_before_confirmed(TimeSpan timeUntilStopped)
        {
            var original = await CreateVerifiedApprenticeship();

            var stoppedOn = original.ApprovedOn.Add(timeUntilStopped);
            await StopApprenticeship(original.CommitmentsApprenticeshipId, stoppedOn);

            var modified = await GetApprenticeships(original.ApprenticeId);
            modified.Should().ContainEquivalentOf(new
            {
                original.ApprenticeId,
                ConfirmedOn = (DateTime?)null,
                StoppedOn = stoppedOn,
            });
        }

        [Test, AutoData]
        public async Task Stopped_after_confirmed(TimeSpan timeUntilStopped)
        {
            var original = await CreateVerifiedApprenticeship();
            await ConfirmApprenticeship(original, new ConfirmationBuilder());

            var stoppedOn = original.ApprovedOn.Add(timeUntilStopped);
            await StopApprenticeship(original.CommitmentsApprenticeshipId, stoppedOn);

            var modified = await GetApprenticeships(original.ApprenticeId);
            modified.Should().ContainEquivalentOf(new
            {
                original.ApprenticeId,
                StoppedOn = stoppedOn,
            });
        }

        [Test, AutoData]
        public async Task Stopped_a_second_time(TimeSpan timeUntilStopped)
        {
            var original = await CreateVerifiedApprenticeship();
            var stoppedOn = original.ApprovedOn.Add(timeUntilStopped);
            await StopApprenticeship(original.CommitmentsApprenticeshipId, stoppedOn);

            stoppedOn = original.ApprovedOn.Add(timeUntilStopped * 2);
            await StopApprenticeship(original.CommitmentsApprenticeshipId, stoppedOn);

            var modified = await GetApprenticeships(original.ApprenticeId);
            modified.Should().ContainEquivalentOf(new
            {
                original.ApprenticeId,
                StoppedOn = stoppedOn,
            });
        }

        [Test, AutoData]
        public async Task Stopped_without_apprenticeship(long id, DateTime stoppedOn)
        {
            var response = await PostStopped(new StoppedApprenticeshipCommand
            {
                CommitmentsApprenticeshipId = id,
                CommitmentsStoppedOn = stoppedOn,
            });

            response.Should().Be404NotFound();
        }
    }
}
