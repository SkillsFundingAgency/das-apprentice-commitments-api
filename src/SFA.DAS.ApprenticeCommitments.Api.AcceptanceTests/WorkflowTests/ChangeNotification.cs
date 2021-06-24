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

            await ChangeApprenticeship(new ChangeEverythingBuilder(apprenticeship));

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

            await ConfirmApprenticeship(apprenticeship, new ConfirmEmployerBuilder());

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

            await ConfirmApprenticeship(apprenticeship, new ConfirmEmployerBuilder());
            await ChangeApprenticeship(new ChangeEverythingBuilder(apprenticeship));

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

            await ConfirmApprenticeship(apprenticeship, new ConfirmEmployerBuilder().AsIncorrect());

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

            await ConfirmApprenticeship(apprenticeship, new ConfirmEmployerBuilder().AsIncorrect());
            await ChangeApprenticeship(new ChangeEverythingBuilder(apprenticeship));

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = true,
            });
        }

        [Test]
        public async Task Confirmed_and_then_changed_and_also_reconfirmed_does_not_show_notification()
        {
            var (apprenticeship, _) = await CreateApprenticeship(client);

            await ConfirmApprenticeship(apprenticeship, new ConfirmEmployerBuilder());
            await ChangeApprenticeship(new ChangeEverythingBuilder(apprenticeship));
            await ConfirmApprenticeship(apprenticeship, new ConfirmEmployerBuilder());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = false,
            });
        }

        [Test]
        public async Task Negatively_confirmed_and_then_changed_and_also_negatively_reconfirmed_does_not_show_notification()
        {
            var (apprenticeship, _) = await CreateApprenticeship(client);

            await ConfirmApprenticeship(apprenticeship, new ConfirmEmployerBuilder());
            await ChangeApprenticeship(new ChangeEverythingBuilder(apprenticeship));
            await ConfirmApprenticeship(apprenticeship, new ConfirmEmployerBuilder());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = false,
            });
        }

        private async Task ConfirmApprenticeship(ApprenticeshipDto apprenticeship, ConfirmEmployerBuilder confirm)
        {
            context.Time.Now = context.Time.Now.AddDays(1);

            apprenticeship = await GetApprenticeship(apprenticeship);

            var r4 = await client.PostValueAsync(
                $"apprentices/{apprenticeship.ApprenticeId}/apprenticeships/{apprenticeship.Id}/statements/{apprenticeship.CommitmentStatementId}/EmployerConfirmation",
                confirm.Build());
            r4.EnsureSuccessStatusCode();
        }

        private async Task ChangeApprenticeship(ChangeEverythingBuilder change)
        {
            context.Time.Now = context.Time.Now.AddDays(1);
            var data = change.ChangedOn(context.Time.Now).Build();
            var r1 = await client.PostValueAsync("apprenticeships/change", data);
            r1.EnsureSuccessStatusCode();
        }

        private async Task<ApprenticeshipDto> GetApprenticeship(ApprenticeshipDto apprenticeship)
        {
            var (r2, apprenticeships) = await client.GetValueAsync<List<ApprenticeshipDto>>(
                $"apprentices/{apprenticeship.ApprenticeId}/apprenticeships");
            r2.EnsureSuccessStatusCode();

            apprenticeships.Should().NotBeEmpty();
            return apprenticeships.Last(); ;
        }

        public async Task<(ApprenticeshipDto, DateTime approvedOn)> CreateApprenticeship(HttpClient client)
        {
            var create = fixture.Build<CreateRegistrationCommand>()
                .With(p => p.Email, (MailAddress adr) => adr.ToString())
                .Create();

            var createResponse = await client.PostValueAsync("apprenticeships", create);
            createResponse.EnsureSuccessStatusCode();

            var verify = fixture.Build<VerifyRegistrationCommand>()
                .With(x => x.ApprenticeId, create.ApprenticeId)
                .With(p => p.Email, create.Email)
                .Create();

            var verifyResponse = await client.PostValueAsync("registrations", verify);
            verifyResponse.EnsureSuccessStatusCode();

            var (getResponse, apprenticeships) = await client.GetValueAsync<List<ApprenticeshipDto>>($"apprentices/{create.ApprenticeId}/apprenticeships");
            getResponse.EnsureSuccessStatusCode();

            context.Time.Now = create.CommitmentsApprovedOn;

            return (apprenticeships[0], create.CommitmentsApprovedOn);
        }
    }

    internal class ConfirmEmployerBuilder
    {
        private bool correctness;

        public ConfirmEmployerBuilder()
        {
            correctness = true;
        }

        internal ConfirmEmployerBuilder AsIncorrect()
        {
            correctness = false;
            return this;
        }

        internal ConfirmEmployerRequest Build() =>
            new ConfirmEmployerRequest { EmployerCorrect = correctness };
    }

    internal class ChangeEverythingBuilder
    {
        private readonly Fixture fixture = new Fixture();
        private IPostprocessComposer<ChangeApprenticeshipCommand> change;

        internal ApprenticeshipDto Apprenticeship { get; }

        internal ChangeEverythingBuilder(ApprenticeshipDto apprenticeship)
        {
            this.Apprenticeship = apprenticeship;
            change = fixture.Build<ChangeApprenticeshipCommand>()
                .Without(x => x.CommitmentsContinuedApprenticeshipId)
                .With(x => x.CommitmentsApprenticeshipId, apprenticeship.CommitmentsApprenticeshipId);
        }

        internal ChangeEverythingBuilder ChangedOn(DateTimeOffset now)
        {
            change = change.With(p => p.CommitmentsApprovedOn, now.DateTime);
            return this;
        }

        internal ChangeApprenticeshipCommand Build()
        {
            return change.Create();
        }

        internal ChangeEverythingBuilder WithUnchangedEmployer()
        {
            change = change
                .With(x => x.EmployerAccountLegalEntityId, Apprenticeship.EmployerAccountLegalEntityId)
                .With(x => x.EmployerName, Apprenticeship.EmployerName);
            return this;
        }
    }
}