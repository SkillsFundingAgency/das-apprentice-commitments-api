using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateAccountCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.VerifyRegistrationCommand;
using System;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    public class ApiFixture
    {
        private protected Fixture fixture;
        private protected HttpClient client;
        private protected TestContext context;

        private protected TimeSpan TimeBetweenActions = TimeSpan.FromDays(2);

        [SetUp]
        public void Setup()
        {
            fixture = new Fixture();
            fixture.Customizations.Add(new EmailPropertyCustomisation());

            var factory = Bindings.Api.CreateApiFactory();
            context = new TestContext();
            _ = new Bindings.Api(context);
            client = factory.CreateClient();
            _ = Bindings.Database.CreateDbContext();
        }

        public async Task<CreateRegistrationCommand> CreateApprenticeship()
        {
            var create = fixture.Build<CreateRegistrationCommand>()
                .Create();

            var response = await client.PostValueAsync("registrations2", create);
            response.Should().Be2XXSuccessful();

            return create;
        }

        protected async Task<CreateAccountCommand> CreateAccount(Guid apprenticeId, string? email = default, DateTime? dateOfBirth = default)
        {
            email ??= fixture.Create<MailAddress>().ToString();
            dateOfBirth ??= fixture.Create<DateTime>();

            var create = fixture.Build<CreateAccountCommand>()
                .With(p => p.ApprenticeId, apprenticeId)
                .With(p => p.Email, (MailAddress adr) => adr.ToString())
                .With(p => p.DateOfBirth, dateOfBirth)
                .Create();

            var response = await client.PostValueAsync("apprentices", create);
            response.Should().Be2XXSuccessful();

            return create;
        }

        protected async Task VerifyRegistration(Guid registrationId, Guid apprenticeId)
        {
            var response = await PostVerifyRegistrationCommand(registrationId, apprenticeId);
            response.Should().Be2XXSuccessful();
        }

        protected async Task<HttpResponseMessage> PostVerifyRegistrationCommand(Guid registrationId, Guid apprenticeId)
        {
            var create = fixture.Build<VerifyRegistrationCommand2>()
                             .With(p => p.RegistrationId, registrationId)
                             .With(p => p.ApprenticeId, apprenticeId)
                             .Create();

            var response = await client.PostValueAsync("apprenticeships2", create);
            return response;
        }
    }
}