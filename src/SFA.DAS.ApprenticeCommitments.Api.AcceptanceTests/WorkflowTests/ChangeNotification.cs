using AutoFixture;
using AutoFixture.Dsl;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Api.Controllers;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.VerifyRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    public class ChangeNotification
    {
        private Fixture fixture;
        private HttpClient client;
        private TestContext context;

        [SetUp]
        public void Setup()
        {
            fixture = new Fixture();

            var factory = Bindings.Api.CreateApiFactory();
            context = new TestContext();
            _ = new Bindings.Api(context);
            client = factory.CreateClient();
            _ = Bindings.Database.CreateDbContext();
        }

        [Test]
        public async Task Incomplete_and_not_changed_does_not_show_notification()
        {
            var (apprenticeship, _) = await CreateApprenticeship(client);

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = false,
            });
        }

        [Test]
        public async Task Incomplete_and_then_changed_shows_notification()
        {
            var (apprenticeship, _) = await CreateApprenticeship(client);

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
            var (apprenticeship, _) = await CreateApprenticeship(client);

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
            var (apprenticeship, _) = await CreateApprenticeship(client);

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
            var (apprenticeship, _) = await CreateApprenticeship(client);

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
            var (apprenticeship, _) = await CreateApprenticeship(client);

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
            var (apprenticeship, _) = await CreateApprenticeship(client);

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship).OnlyChangeEmployer());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = true,
            });
        }

        [Test]
        public async Task One_section_changed_and_then_confirmed_hides_notification()
        {
            var (apprenticeship, _) = await CreateApprenticeship(client);

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
        public async Task Confirmed_and_then_changed_and_also_reconfirmed_does_not_show_notification()
        {
            var (apprenticeship, _) = await CreateApprenticeship(client);

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
            var (apprenticeship, _) = await CreateApprenticeship(client);

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
            var (apprenticeship, _) = await CreateApprenticeship(client);

            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder().AsIncomplete());
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship));
            await ConfirmApprenticeship(apprenticeship, new ConfirmationBuilder().AsIncomplete());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = false,
            });
        }

        private async Task ConfirmApprenticeship(ApprenticeshipDto apprenticeship, ConfirmationBuilder confirm)
        {
            context.Time.Now = context.Time.Now.AddDays(1);

            apprenticeship = await GetApprenticeship(apprenticeship);

            foreach (var payload in confirm.BuildAll())
            {
                var r4 = await client.PostValueAsync(
                    $"apprentices/{apprenticeship.ApprenticeId}/apprenticeships/{apprenticeship.Id}/revisions/{apprenticeship.CommitmentStatementId}/{payload.Item1}",
                    payload.Item2);
                r4.Should().Be2XXSuccessful();
            }
        }

        private async Task ChangeApprenticeship(ChangeBuilder change)
        {
            context.Time.Now = context.Time.Now.AddDays(1);
            var data = change.ChangedOn(context.Time.Now).Build();
            var r1 = await client.PostValueAsync("apprenticeships/change", data);
            r1.Should().Be2XXSuccessful();
        }

        private async Task<ApprenticeshipDto> GetApprenticeship(ApprenticeshipDto apprenticeship)
        {
            var (r2, apprenticeships) = await client.GetValueAsync<List<ApprenticeshipDto>>(
                $"apprentices/{apprenticeship.ApprenticeId}/apprenticeships");
            r2.Should().Be2XXSuccessful();

            apprenticeships.Should().NotBeEmpty();
            return apprenticeships.Last(); ;
        }

        public async Task<(ApprenticeshipDto, DateTime approvedOn)> CreateApprenticeship(HttpClient client)
        {
            var create = fixture.Build<CreateRegistrationCommand>()
                .With(p => p.Email, (MailAddress adr) => adr.ToString())
                .Create();

            var createResponse = await client.PostValueAsync("apprenticeships", create);
            createResponse.Should().Be2XXSuccessful();

            var verify = fixture.Build<VerifyRegistrationCommand>()
                .With(x => x.ApprenticeId, create.ApprenticeId)
                .With(p => p.Email, create.Email)
                .With(p => p.DateOfBirth, create.DateOfBirth)
                .Create();

            var verifyResponse = await client.PostValueAsync("registrations", verify);
            verifyResponse.Should().Be2XXSuccessful();

            var (getResponse, apprenticeships) = await client.GetValueAsync<List<ApprenticeshipDto>>($"apprentices/{create.ApprenticeId}/apprenticeships");
            getResponse.Should().Be2XXSuccessful();

            context.Time.Now = create.CommitmentsApprovedOn;

            return (apprenticeships[0], create.CommitmentsApprovedOn);
        }
    }
}