﻿using AutoFixture;
using AutoFixture.Dsl;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.ApprenticeCommitments.Api.Controllers;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    public class ChangeNotificationFixture : ApiFixture
    {
        public async Task<(ApprenticeshipDto, DateTime approvedOn)> CreateVerifiedApprenticeship()
        {
            var approval = await CreateRegistration();
            var account = await CreateAccount(approval);
            await VerifyRegistration(approval.ApprenticeId, approval.ApprenticeId);

            var (response, apprenticeships) = await client.GetValueAsync<List<ApprenticeshipDto>>($"apprentices/{approval.ApprenticeId}/apprenticeships");
            response.Should().Be2XXSuccessful();

            context.Time.Now = approval.CommitmentsApprovedOn;

            return (apprenticeships[0], approval.CommitmentsApprovedOn);
        }

        protected async Task<ApprenticeshipDto> GetApprenticeship(ApprenticeshipDto apprenticeship)
        {
            var (r2, apprenticeships) = await client.GetValueAsync<ApprenticeshipDto>(
                $"apprentices/{apprenticeship.ApprenticeId}/apprenticeships/{apprenticeship.CommitmentStatementId}");
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
            var r1 = await client.PostValueAsync("apprenticeships/change", data);
            r1.EnsureSuccessStatusCode();
        }

        private protected async Task ConfirmApprenticeship(ApprenticeshipDto apprenticeship, ConfirmationBuilder confirm)
        {
            context.Time.Now = context.Time.Now.Add(TimeBetweenActions);

            apprenticeship = await GetApprenticeship(apprenticeship);

            foreach (var payload in confirm.BuildAll())
            {
                var r4 = await client.PostValueAsync(
                    $"apprentices/{apprenticeship.ApprenticeId}/apprenticeships/{apprenticeship.Id}/revisions/{apprenticeship.CommitmentStatementId}/{payload.Item1}",
                    payload.Item2);
                r4.EnsureSuccessStatusCode();
            }
        }
    }

    internal class ConfirmationBuilder
    {
        private bool? employerCorrect = true;
        private bool? providerCorrect = true;
        private bool? detailsCorrect = true;

        internal ConfirmationBuilder AsIncomplete()
        {
            employerCorrect = providerCorrect = detailsCorrect = false;
            return this;
        }

        internal List<(string, object)> BuildAll()
        {
            var all = new List<(string, object)>();
            if (employerCorrect != null) all.Add(("EmployerConfirmation", new ConfirmEmployerRequest { EmployerCorrect = employerCorrect.Value }));
            if (providerCorrect != null) all.Add(("TrainingProviderConfirmation", new ConfirmTrainingProviderRequest { TrainingProviderCorrect = providerCorrect.Value }));
            if (detailsCorrect != null) all.Add(("ApprenticeshipDetailsConfirmation", new ConfirmApprenticeshipRequest { ApprenticeshipCorrect = detailsCorrect.Value }));
            return all;
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