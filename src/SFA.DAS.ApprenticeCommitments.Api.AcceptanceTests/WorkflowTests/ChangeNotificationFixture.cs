using AutoFixture;
using AutoFixture.Dsl;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.ApprenticeCommitments.Api.Controllers;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    public class ChangeNotificationFixture : ApiFixture
    {
        protected new async Task<ApprenticeshipDto> GetApprenticeship(ApprenticeshipDto apprenticeship)
        {
            var (r2, apprenticeships) = await client.GetValueAsync<ApprenticeshipDto>(
                $"apprentices/{apprenticeship.ApprenticeId}/apprenticeships/{apprenticeship.RevisionId}");
            r2.EnsureSuccessStatusCode();

            apprenticeships.Should().NotBeNull();
            return apprenticeships;
        }

        protected async Task ViewApprenticeship(ApprenticeshipDto apprenticeship)
        {
            var patch = new JsonPatchDocument<ApprenticeshipDto>()
                .Replace(a => a.LastViewed, context.Time.Now);

            var r3 = await client.PatchAsync(
                $"apprentices/{apprenticeship.ApprenticeId}/apprenticeships/{apprenticeship.Id}",
                patch.GetStringContent());

            r3.EnsureSuccessStatusCode();
        }

        private protected async Task ChangeApprenticeship(ChangeBuilder change)
        {
            context.Time.Now = context.Time.Now.Add(TimeBetweenActions);
            var data = change.ChangedOn(context.Time.Now).Build();
            var r1 = await client.PutValueAsync("registrations", data);
            r1.EnsureSuccessStatusCode();
        }
    }

    internal class ConfirmationBuilder
    {
        private bool? employerCorrect = true;
        private bool? providerCorrect = true;
        private bool? detailsCorrect = true;
        private bool confirmCompletely = false;

        internal ConfirmationBuilder AsIncomplete()
        {
            employerCorrect = providerCorrect = detailsCorrect = false;
            return this;
        }

        internal List<Confirmations> BuildAll()
        {
            var confirmations = new Confirmations
            {
                HowApprenticeshipDeliveredCorrect = true,
                RolesAndResponsibilitiesCorrect = true,
            };

            if (employerCorrect != null) confirmations.EmployerCorrect = employerCorrect.Value;
            if (providerCorrect != null) confirmations.TrainingProviderCorrect = providerCorrect.Value;
            if (detailsCorrect != null) confirmations.ApprenticeshipDetailsCorrect = detailsCorrect.Value;
            if (confirmCompletely) confirmations.ApprenticeshipCorrect = true;

            return new List<Confirmations> { confirmations };
        }

        internal ConfirmationBuilder ConfirmOnlyEmployer()
        {
            providerCorrect = detailsCorrect = null;
            employerCorrect = true;
            return this;
        }

        internal ConfirmationBuilder ConfirmOnlyProvider()
        {
            employerCorrect = detailsCorrect = null;
            providerCorrect = true;
            return this;
        }

        internal ConfirmationBuilder ConfirmCompletely()
        {
            confirmCompletely = true;
            return this;
        }
    }

    internal class ChangeBuilder
    {
        private readonly Fixture fixture = new Fixture();
        private IPostprocessComposer<ChangeApprenticeshipCommand> change;

        internal ApprenticeshipDto Apprenticeship { get; }

        internal ChangeBuilder(ApprenticeshipDto apprenticeship)
        {
            this.Apprenticeship = apprenticeship;
            change = fixture.Build<ChangeApprenticeshipCommand>()
                .Without(x => x.CommitmentsContinuedApprenticeshipId)
                .With(x => x.CommitmentsApprenticeshipId, apprenticeship.CommitmentsApprenticeshipId);
        }

        internal ChangeBuilder ChangedOn(DateTimeOffset now)
        {
            change = change.With(p => p.CommitmentsApprovedOn, now.DateTime);
            return this;
        }

        internal ChangeApprenticeshipCommand Build()
        {
            return change.Create();
        }

        internal ChangeBuilder OnlyChangeEmployer()
        {
            return WithUnchangedProvider().WithUnchangedCourse();
        }

        internal ChangeBuilder OnlyChangeProvider()
        {
            return WithUnchangedEmployer().WithUnchangedCourse();
        }

        internal ChangeBuilder WithUnchangedProvider()
        {
            change = change
                .With(x => x.TrainingProviderId, Apprenticeship.TrainingProviderId)
                .With(x => x.TrainingProviderName, Apprenticeship.TrainingProviderName);
            return this;
        }

        internal ChangeBuilder WithUnchangedCourse()
        {
            change = change
                .With(x => x.CourseName, Apprenticeship.CourseName)
                .With(x => x.CourseLevel, Apprenticeship.CourseLevel)
                .With(x => x.CourseOption, Apprenticeship.CourseOption)
                .With(x => x.PlannedStartDate, Apprenticeship.PlannedStartDate)
                .With(x => x.PlannedEndDate, Apprenticeship.PlannedEndDate);
            return this;
        }

        internal ChangeBuilder WithUnchangedEmployer()
        {
            change = change
                .With(x => x.EmployerAccountLegalEntityId, Apprenticeship.EmployerAccountLegalEntityId)
                .With(x => x.EmployerName, Apprenticeship.EmployerName);
            return this;
        }
    }
}