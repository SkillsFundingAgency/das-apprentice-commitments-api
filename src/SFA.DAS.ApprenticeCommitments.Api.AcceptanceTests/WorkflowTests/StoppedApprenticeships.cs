using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Commands.StoppedApprenticeshipCommand;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    internal class StoppedApprenticeships : ApiFixture
    {
        [Test]
        public async Task Stopped_before_confirmed()
        {
            var stoppedOn = DateTime.Now;

            var apprenticeship = await CreateVerifiedApprenticeship();

            var response = await PostStopped(new StoppedApprenticeshipCommand
            {
                CommitmentsApprenticeshipId = apprenticeship.CommitmentsApprenticeshipId,
                CommitmentsStoppedOn = stoppedOn,
            });

            response.Should().Be2XXSuccessful();

            var ap2 = await GetApprenticeships(apprenticeship.ApprenticeId);

            ap2.Should().ContainEquivalentOf(new
            {
                apprenticeship.ApprenticeId,
                StoppedOn = stoppedOn,
            });
        }

        private Task<HttpResponseMessage> PostStopped(StoppedApprenticeshipCommand command)
            => client.PostValueAsync(
                $"registrations/stopped/{command.CommitmentsApprenticeshipId}", command);
    }
}
