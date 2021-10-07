﻿using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Queries.ApprenticeshipsQuery;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                DisplayChangeNotification = false,
            });
        }

        [Test]
        public async Task Incomplete_and_then_changed_shows_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ChangeApprenticeship(new ChangeBuilder(apprenticeship));

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = true,
            });
        }

        [Test]
        public async Task Confirmed_and_not_changed_does_not_show_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = false,
            });
        }

        [Test]
        public async Task Confirmed_and_then_changed_shows_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship));

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = true,
            });
        }

        [Test]
        public async Task Negatively_confirmed_and_not_changed_does_not_show_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = false,
            });
        }

        [Test]
        public async Task Negatively_confirmed_and_then_changed_shows_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship));

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = true,
            });
        }

        [Test]
        public async Task One_section_confirmed_and_then_all_changed_shows_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship).OnlyChangeEmployer());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = true,
            });
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
                DisplayChangeNotification = false,
            });
        }

        [Test]
        public async Task Provider_section_changed_and_then_confirmed_hides_notification()
        {
            var apprenticeship = await CreateVerifiedApprenticeship();

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder().ConfirmOnlyProvider());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship).OnlyChangeProvider());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = true,
            });
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
                DisplayChangeNotification = false,
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
                DisplayChangeNotification = false,
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
                DisplayChangeNotification = false,
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
                DisplayChangeNotification = false,
            });
        }

        private async Task ChangeApprenticeship(ChangeBuilder change)
        {
            context.Time.Now = context.Time.Now.AddDays(1);
            var data = change.ChangedOn(context.Time.Now).Build();
            var r1 = await client.PutValueAsync("registrations", data);
            r1.Should().Be2XXSuccessful();
        }
    }
}