using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Queries.ApprenticeshipsQuery;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    public class ChangeNotification : ApiFixture
    {
        [Test]
        public async Task Incomplete_and_not_changed_does_not_show_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                ChangeOfCircumstanceNotifications = ChangeOfCircumstanceNotifications.None,
            });
        }

        [Test]
        public async Task Incomplete_and_then_changed_shows_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();
            await ViewApprenticeship(apprenticeship);

            await ChangeApprenticeship(new ChangeBuilder(apprenticeship));

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.ChangeOfCircumstanceNotifications.Should().HaveFlag(ChangeOfCircumstanceNotifications.EmployerDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().HaveFlag(ChangeOfCircumstanceNotifications.ProviderDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().HaveFlag(ChangeOfCircumstanceNotifications.ApprenticeshipDetailsChanged);
        }

        [Test]
        public async Task Confirmed_and_not_changed_does_not_show_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                ChangeOfCircumstanceNotifications = ChangeOfCircumstanceNotifications.None,
            });
        }

        [Test]
        public async Task Confirmed_and_then_changed_shows_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();
            await ViewApprenticeship(apprenticeship);

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship));
            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.ChangeOfCircumstanceNotifications.Should().HaveFlag(ChangeOfCircumstanceNotifications.EmployerDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().HaveFlag(ChangeOfCircumstanceNotifications.ProviderDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().HaveFlag(ChangeOfCircumstanceNotifications.ApprenticeshipDetailsChanged);
        }

        [Test]
        public async Task Negatively_confirmed_and_not_changed_does_not_show_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                ChangeOfCircumstanceNotifications = ChangeOfCircumstanceNotifications.None,
            });
        }

        [Test]
        public async Task Negatively_confirmed_and_then_changed_shows_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();
            await ViewApprenticeship(apprenticeship);

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship));

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.ChangeOfCircumstanceNotifications.Should().HaveFlag(ChangeOfCircumstanceNotifications.EmployerDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().HaveFlag(ChangeOfCircumstanceNotifications.ProviderDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().HaveFlag(ChangeOfCircumstanceNotifications.ApprenticeshipDetailsChanged);
        }

        [Test]
        public async Task One_section_confirmed_and_then_all_changed_shows_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();
            await ViewApprenticeship(apprenticeship);

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship).OnlyChangeEmployer());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.ChangeOfCircumstanceNotifications.Should().HaveFlag(ChangeOfCircumstanceNotifications.EmployerDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().NotHaveFlag(ChangeOfCircumstanceNotifications.ProviderDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().NotHaveFlag(ChangeOfCircumstanceNotifications.ApprenticeshipDetailsChanged);
        }

        [Test]
        public async Task Multiple_changes_occur_sequentially_then_apprentice_is_notifiied_only_of_differences_between_last_viewed_and_latest()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();
            await ViewApprenticeship(apprenticeship);
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship).ChangedOn(DateTimeOffset.Now.AddHours(1)));
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship).OnlyChangeEmployer().ChangedOn(DateTimeOffset.Now.AddHours(2)));

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.ChangeOfCircumstanceNotifications.Should().HaveFlag(ChangeOfCircumstanceNotifications.EmployerDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().NotHaveFlag(ChangeOfCircumstanceNotifications.ProviderDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().NotHaveFlag(ChangeOfCircumstanceNotifications.ApprenticeshipDetailsChanged);
        }

        [Test]
        public async Task Multiple_changes_occur_sequentially_then_apprentice_is_not_told_as_they_havent_viewed()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship).ChangedOn(DateTimeOffset.Now.AddHours(1)));

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.ChangeOfCircumstanceNotifications.Should().NotHaveFlag(ChangeOfCircumstanceNotifications.EmployerDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().NotHaveFlag(ChangeOfCircumstanceNotifications.ProviderDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().NotHaveFlag(ChangeOfCircumstanceNotifications.ApprenticeshipDetailsChanged);
        }

        [Test]
        public async Task Employer_section_changed_and_then_confirmed_hides_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder().ConfirmOnlyEmployer());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship).OnlyChangeEmployer());
            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder().ConfirmOnlyEmployer());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                ChangeOfCircumstanceNotifications = ChangeOfCircumstanceNotifications.None,
            });
        }

        [Test]
        public async Task Provider_section_changed_and_then_confirmed_hides_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();
            await ViewApprenticeship(apprenticeship);

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder().ConfirmOnlyProvider());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship).OnlyChangeProvider());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.ChangeOfCircumstanceNotifications.Should().NotHaveFlag(ChangeOfCircumstanceNotifications.EmployerDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().HaveFlag(ChangeOfCircumstanceNotifications.ProviderDetailsChanged);
            retrieved.ChangeOfCircumstanceNotifications.Should().NotHaveFlag(ChangeOfCircumstanceNotifications.ApprenticeshipDetailsChanged);
        }

        [Test]
        public async Task Provider_section_confirmed_and_then_changed_shows_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder().ConfirmOnlyProvider());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship).OnlyChangeProvider());
            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder().ConfirmOnlyProvider());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                ChangeOfCircumstanceNotifications = ChangeOfCircumstanceNotifications.None,
            });
        }

        [Test]
        public async Task Confirmed_and_then_changed_and_also_reconfirmed_does_not_show_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship));
            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                ChangeOfCircumstanceNotifications = ChangeOfCircumstanceNotifications.None,
            });
        }

        [Test]
        public async Task Negatively_confirmed_and_then_changed_and_also_reconfirmed_does_not_show_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder().AsIncomplete());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship));
            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                ChangeOfCircumstanceNotifications = ChangeOfCircumstanceNotifications.None,
            });
        }

        [Test]
        public async Task Negatively_confirmed_and_then_changed_and_negatively_confirmed_again_does_not_show_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder().AsIncomplete());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship));
            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder().AsIncomplete());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                ChangeOfCircumstanceNotifications = ChangeOfCircumstanceNotifications.None,
            });
        }

        private async Task ConfirmApprenticeship(ApprenticeshipDto apprenticeship, ConfirmationBuilder confirm)
        {
            context.Time.Now = context.Time.Now.AddDays(1);

            apprenticeship = await GetApprenticeship(apprenticeship);

            foreach (var payload in confirm.BuildAll())
            {
                var r4 = await client.PostValueAsync(
                    $"apprentices/{apprenticeship.ApprenticeId}/apprenticeships/{apprenticeship.Id}/revisions/{apprenticeship.RevisionId}/{payload.Item1}",
                    payload.Item2);
                r4.Should().Be2XXSuccessful();
            }
        }

        private async Task ChangeApprenticeship(ChangeBuilder change)
        {
            context.Time.Now = context.Time.Now.AddDays(1);
            var data = change.ChangedOn(context.Time.Now).Build();
            var r1 = await client.PutValueAsync("registrations", data);
            r1.Should().Be2XXSuccessful();
        }

        private async Task<ApprenticeshipDto> GetApprenticeship(ApprenticeshipDto apprenticeship)
        {
            var (r2, apprenticeships) = await client.GetValueAsync<ApprenticeshipsResponse>(
                $"apprentices/{apprenticeship.ApprenticeId}/apprenticeships");
            r2.Should().Be2XXSuccessful();

            apprenticeships.Apprenticeships.Should().NotBeEmpty();
            return apprenticeships.Apprenticeships.Last();
        }

        protected async Task ViewApprenticeship(ApprenticeshipDto apprenticeship)
        {
            var patch = new JsonPatchDocument<ApprenticeshipDto>()
                .Replace(a => a.LastViewed, context.Time.Now);

            var r3 = await client.PatchAsync(
                $"apprentices/{apprenticeship.ApprenticeId}/apprenticeships/{apprenticeship.Id}/revisions/{apprenticeship.RevisionId}",
                patch.GetStringContent());

            r3.EnsureSuccessStatusCode();
        }
    }
}