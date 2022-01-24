using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System;
using System.Linq;
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
                StoppedReceivedOn = context.Time.Now,
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
                StoppedReceivedOn = context.Time.Now,
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
                StoppedReceivedOn = context.Time.Now,
            });
        }

        [Test, AutoData]
        public async Task Stopped_after_change_of_circumstance()
        {
            // Given
            var original = await CreateRegistration();
            var apprenticeship = await VerifyRegistration(original);

            var coc = fixture.Create<ChangeRegistrationCommand>();
            coc.CommitmentsContinuedApprenticeshipId = original.CommitmentsApprenticeshipId;
            coc.CommitmentsApprovedOn = original.CommitmentsApprovedOn.AddDays(1);
            await ChangeOfCircumstances(coc);

            // When
            var stoppedOn = original.CommitmentsApprovedOn.AddDays(15);
            context.Time.Now = stoppedOn;
            await StopApprenticeship(coc.CommitmentsApprenticeshipId, stoppedOn);

            // Then
            //var reg = await GetRegistration(original.RegistrationId);
            var rev = await GetApprenticeship(apprenticeship.ApprenticeId);
            Database.Registrations.Should().ContainEquivalentOf(new
            {
                original.CommitmentsApprenticeshipId,
                StoppedReceivedOn = (DateTime?)null,
            });
            rev.Should().BeEquivalentTo(new
            {
                coc.CommitmentsApprenticeshipId,
                StoppedReceivedOn = (DateTime?)stoppedOn,
            });
        }

        [Test, AutoData]
        public async Task Stopped_known_approval_without_apprenticeship(DateTime stoppedOn)
        {
            var original = await CreateRegistration();

            var response = await PostStopped(original.CommitmentsApprenticeshipId, stoppedOn);
            response.Should().Be200Ok();

            var modified = await GetRegistration(original.RegistrationId);
            modified.Should().BeEquivalentTo(new
            {
                original.RegistrationId,
                //StoppedReceivedOn = context.Time.Now, // TODO reconcile RegistrationResponse with RegistrationDto
            });
        }

        [Test, AutoData]
        public async Task Stopped_unknown_approval(long id, DateTime stoppedOn)
        {
            var response = await PostStopped(id, stoppedOn);
            response.Should().Be404NotFound();
        }

        [Test, AutoData]
        [Ignore("scenario not defined yet")]
        public async Task Stopped_unmatched_apprenticeship()
        {
            var registration = await CreateRegistration();

            await StopApprenticeship(
                registration.CommitmentsApprenticeshipId,
                registration.CommitmentsApprovedOn.AddDays(1));

            // create apprenticeship and revision and immediately stop it???
        }

        [Test, AutoData]
        public async Task Publish_event(DateTime stoppedOn)
        {
            var apprenticeship = await CreateVerifiedApprenticeship();
            await StopApprenticeship(apprenticeship.CommitmentsApprenticeshipId, stoppedOn);

            context.PublishedNServiceBusEvents
                .Select(x => x.Event as ApprenticeshipStoppedEvent)
                .Should().ContainEquivalentOf(new
                {
                    ApprenticeshipId = apprenticeship.Id,
                    apprenticeship.ApprenticeId,
                    apprenticeship.EmployerName,
                    apprenticeship.CourseName,
                });
        }

        [Test, AutoData]
        public async Task Publish_event_for_unmatched_approval(DateTime stoppedOn)
        {
            var approval = await CreateRegistration();
            await StopApprenticeship(approval.CommitmentsApprenticeshipId, stoppedOn);

            context.PublishedNServiceBusEvents
                .Select(x => x.Event as ApprenticeshipStoppedEvent)
                .Should().ContainEquivalentOf(new
                {
                    ApprenticeshipId = (long?)null,
                    ApprenticeId = (Guid?)null,
                    approval.EmployerName,
                    approval.CourseName,
                });
        }

        [Test, AutoData]
        public async Task Does_not_publish_event_for_unknown_approval(long commitmentsApprenticeshipId, DateTime stoppedOn)
        {
            await PostStopped(commitmentsApprenticeshipId, stoppedOn);

            context.PublishedNServiceBusEvents
                .Select(x => x.Event as ApprenticeshipStoppedEvent)
                .Should().BeEmpty();
        }

        [Test, AutoData]
        public async Task Matching_a_stopped_registraiton_creates_a_stopped_apprenticeship(DateTime stoppedOn)
        {
            // Given
            var approval = await CreateRegistration();

            context.Time.Now = stoppedOn;
            await PostStopped(approval.CommitmentsApprenticeshipId);

            // When
            var account = await CreateAccount(approval);
            await VerifyRegistration(approval, account);

            // Then
            var apprenticeships = await GetApprenticeships(account.ApprenticeId);
            apprenticeships.Should().ContainEquivalentOf(new
            {
                account.ApprenticeId,
                approval.CommitmentsApprenticeshipId,
                StoppedReceivedOn = stoppedOn,
                ConfirmedOn = (DateTime?)null,
            });
        }
    }
}