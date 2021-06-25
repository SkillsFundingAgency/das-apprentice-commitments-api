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
            var apibinding = new Bindings.Api(context);
            client = factory.CreateClient();
            var db = Bindings.Database.CreateDbContext();
        }

        [Test]
        public async Task No_changes_and_not_confirmed_has_no_notification()
        {
            var (apprenticeship, _) = await CreateApprenticeship(client);

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = false,
            });
        }

        [Test]
        public async Task Changes_after_not_confirmed_has_notification()
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
        public async Task Changes_after_section_confirmed_has_notification()
        {
            var (apprenticeship, _) = await CreateApprenticeship(client);

            await ConfirmApprenticeship(new ConfirmEmployerBuilder(apprenticeship));
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship));

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = true,
            });
        }

        [Test]
        public async Task Irrelevant_changes_after_section_confirmed_has_no_notification()
        {
            var (apprenticeship, _) = await CreateApprenticeship(client);

            await ConfirmApprenticeship(new ConfirmEmployerBuilder(apprenticeship));
            await ChangeApprenticeship(new ChangeBuilder(apprenticeship).WithUnchangedEmployer());

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = false,
            });
        }

        [Test]
        public async Task No_changes_after_section_confirmed_are_has_no_notification()
        {
            var (apprenticeship, _) = await CreateApprenticeship(client);

            await ConfirmApprenticeship(new ConfirmEmployerBuilder(apprenticeship));

            var retrieved = await GetApprenticeship(apprenticeship);
            retrieved.Should().BeEquivalentTo(new
            {
                DisplayChangeNotification = false,
            });
        }

        private async Task ConfirmApprenticeship(ConfirmEmployerBuilder confirm)
        {
            context.Time.Now = context.Time.Now.AddDays(1);
            var r4 = await client.PostValueAsync(
                $"apprentices/{confirm.ApprenticeId}/apprenticeships/{confirm.ApprenticeshipId}/statements/{confirm.CommitmentStatementId}/EmployerConfirmation",
                confirm.Build());
            r4.EnsureSuccessStatusCode();
        }

        private async Task ChangeApprenticeship(ChangeBuilder change)
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
        private readonly ApprenticeshipDto apprenticeship;
        private readonly ConfirmEmployerRequest confirmation;

        public ConfirmEmployerBuilder(ApprenticeshipDto apprenticeship)
        {
            this.apprenticeship = apprenticeship;
            confirmation = new ConfirmEmployerRequest { EmployerCorrect = true };
        }

        public Guid ApprenticeId => apprenticeship.ApprenticeId;
        public long ApprenticeshipId => apprenticeship.Id;
        public long CommitmentStatementId => apprenticeship.CommitmentStatementId;

        internal ConfirmEmployerRequest Build() => confirmation;
    }

    public class ChangeBuilder
    {
        private readonly Fixture fixture = new Fixture();
        private readonly ApprenticeshipDto apprenticeship;
        private IPostprocessComposer<ChangeApprenticeshipCommand> change;

        public ChangeBuilder(ApprenticeshipDto apprenticeship)
        {
            this.apprenticeship = apprenticeship;
            change = fixture.Build<ChangeApprenticeshipCommand>()
                .Without(x => x.CommitmentsContinuedApprenticeshipId)
                .With(x => x.CommitmentsApprenticeshipId, apprenticeship.CommitmentsApprenticeshipId);
        }

        internal ChangeBuilder ChangedOn(DateTimeOffset now)
        {
            change = change.With(p => p.CommitmentsApprovedOn, now.DateTime);
            return this;
        }

        public ChangeApprenticeshipCommand Build()
        {
            return change.Create();
        }

        internal ChangeBuilder WithUnchangedEmployer()
        {
            change = change
                .With(x => x.EmployerAccountLegalEntityId, apprenticeship.EmployerAccountLegalEntityId)
                .With(x => x.EmployerName, apprenticeship.EmployerName);
            return this;
        }
    }
}