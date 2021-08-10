﻿using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Api.Controllers;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    public class RegisterRenewConfirm : ApiFixture
    {
        [Test]
        public async Task ChangingApprenticeshipCreatesNewCommitmentStatementWhichIsLatest()
        {
            var create = await CreateRegistration();
            var account = await CreateAccount(create);
            await VerifyRegistration(create.RegistrationId, account.ApprenticeId);
            var apprenticeships = await GetApprenticeships(account.ApprenticeId);
            var apprenticeshipId = apprenticeships[0].Id;

            var r4 = await client.PostValueAsync(
                $"apprentices/{account.ApprenticeId}/apprenticeships/{apprenticeshipId}/revisions/{apprenticeships[0].CommitmentStatementId}/EmployerConfirmation",
                new ConfirmEmployerRequest { EmployerCorrect = true });
            r4.Should().Be2XXSuccessful();

            var change = fixture.Build<ChangeApprenticeshipCommand>()
                .Without(x => x.CommitmentsContinuedApprenticeshipId)
                .With(x => x.CommitmentsApprenticeshipId, create.CommitmentsApprenticeshipId)
                .With(p => p.CommitmentsApprovedOn, (int days) => create.CommitmentsApprovedOn.AddDays(days))
                .Create();

            var r5 = await client.PostValueAsync("apprenticeships/change", change);
            r5.EnsureSuccessStatusCode();

            apprenticeships = await GetApprenticeships(account.ApprenticeId);

            apprenticeships
                .Should().ContainEquivalentOf(new
                {
                    change.CommitmentsApprenticeshipId,
                    change.CourseName,
                });
        }

        [Test]
        public async Task ConfirmingCommitmentStatementConcurrentToApprovedChangeConfirmsCorrectRevision()
        {
            var create = await CreateRegistration();
            var account = await CreateAccount(create, apprenticeId: create.RegistrationId);
            await VerifyRegistration(create.RegistrationId, create.RegistrationId);

            var apprenticeships = await GetApprenticeships(create.RegistrationId);
            var apprenticeshipId = apprenticeships[0].Id;
            var csId = apprenticeships[0].CommitmentStatementId;

            var change = fixture.Build<ChangeApprenticeshipCommand>()
                .With(x => x.CommitmentsApprenticeshipId, create.CommitmentsApprenticeshipId)
                .Without(x => x.CommitmentsContinuedApprenticeshipId)
                .With(p => p.CommitmentsApprovedOn, (int days) => create.CommitmentsApprovedOn.AddDays(days))
                .Create();

            var r5 = await client.PostValueAsync("apprenticeships/change", change);
            r5.EnsureSuccessStatusCode();

            var r4 = await client.PostValueAsync(
                $"apprentices/{create.RegistrationId}/apprenticeships/{apprenticeshipId}/revisions/{csId}/EmployerConfirmation",
                new ConfirmEmployerRequest { EmployerCorrect = true });
            r4.StatusCode.Should().Be(HttpStatusCode.OK);

            var (r6, apprenticeships2) = await client.GetValueAsync<List<ApprenticeshipDto>>($"apprentices/{create.RegistrationId}/apprenticeships");
            r5.EnsureSuccessStatusCode();

            apprenticeships2.Should()
                .ContainEquivalentOf(new
                {
                    Id = apprenticeshipId,
                    CommitmentStatementId = csId + 1,
                    EmployerCorrect = (bool?)null,
                });
        }
    }
}