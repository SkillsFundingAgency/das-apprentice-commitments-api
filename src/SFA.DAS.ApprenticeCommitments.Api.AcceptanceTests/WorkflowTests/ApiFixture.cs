using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateApprenticeAccountCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateApprenticeshipFromRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Application.Queries.RegistrationQuery;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    public class ApiFixture
    {
        private protected Fixture fixture = null!;
        private protected TestContext context = null!;
        private protected HttpClient client = null!;
        protected ApprenticeCommitmentsDbContext Database { get; private set; } = null!;

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
            Database = Bindings.Database.CreateDbContext();
        }

        public async Task<CreateRegistrationCommand> CreateRegistration()
        {
            var create = fixture.Build<CreateRegistrationCommand>()
                .Create();

            var response = await PostCreateRegistrationCommand(create);
            response.Should().Be2XXSuccessful();

            return create;
        }

        protected async Task<HttpResponseMessage> PostCreateRegistrationCommand(CreateRegistrationCommand create)
        {
            return await client.PostValueAsync("registrations", create);
        }

        protected async Task<RegistrationResponse> GetRegistration(Guid apprenticeId)
        {
            var (response, registration) = await client.GetValueAsync<RegistrationResponse>($"registrations/{apprenticeId}");
            response.Should().Be200Ok();
            return registration;
        }

        protected async Task<CreateApprenticeAccountCommand> CreateAccount(CreateRegistrationCommand approval,
            Guid? apprenticeId = default, MailAddress? email = default, DateTime? dateOfBirth = default)
        {
            email ??= new MailAddress(approval.Email);
            dateOfBirth ??= approval.DateOfBirth;

            var create = fixture.Build<CreateApprenticeAccountCommand>()
                .With(p => p.ApprenticeId, (Guid id) => apprenticeId ?? id)
                .With(p => p.Email, email.ToString())
                .With(p => p.DateOfBirth, dateOfBirth)
                .With(p => p.FirstName, approval.FirstName)
                .With(p => p.LastName, approval.LastName)
                .Create();

            var response = await PostCreateAccountCommand(create);
            response.Should().Be2XXSuccessful();

            return create;
        }

        protected async Task<ApprenticeshipDto> CreateVerifiedApprenticeship()
        {
            var approval = await CreateRegistration();
            var account = await CreateAccount(approval);
            await VerifyRegistration(approval.RegistrationId, account.ApprenticeId);
            var apprenticeship = await GetApprenticeships(account.ApprenticeId);
            context.Time.Now = approval.CommitmentsApprovedOn;
            return apprenticeship[0];
        }

        protected async Task<ApprenticeDto> GetApprentice(Guid apprenticeId)
        {
            var (response, apprentice) = await client.GetValueAsync<ApprenticeDto>($"apprentices/{apprenticeId}");
            response.Should().Be200Ok();
            return apprentice;
        }

        protected async Task<HttpResponseMessage> PostCreateAccountCommand(CreateApprenticeAccountCommand create)
        {
            return await client.PostValueAsync("apprentices", create);
        }

        protected async Task VerifyRegistration(Guid registrationId, Guid apprenticeId)
        {
            var response = await PostVerifyRegistrationCommand(registrationId, apprenticeId);
            response.Should().Be2XXSuccessful();
        }

        protected async Task<HttpResponseMessage> PostVerifyRegistrationCommand(Guid registrationId, Guid apprenticeId)
        {
            var create = fixture.Build<CreateApprenticeshipFromRegistrationCommand>()
                             .With(p => p.RegistrationId, registrationId)
                             .With(p => p.ApprenticeId, apprenticeId)
                             .Create();

            var response = await client.PostValueAsync("apprenticeships", create);
            return response;
        }

        protected async Task<List<ApprenticeshipDto>> GetApprenticeships(Guid apprenticeId)
        {
            var (response, apprenticeships) = await client.GetValueAsync<List<ApprenticeshipDto>>($"apprentices/{apprenticeId}/apprenticeships");
            response.Should().Be200Ok();
            return apprenticeships;
        }
    }
}