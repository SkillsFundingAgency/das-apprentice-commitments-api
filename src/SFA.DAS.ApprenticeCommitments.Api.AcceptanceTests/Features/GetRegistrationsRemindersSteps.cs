using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Application.Queries.RegistrationRemindersQuery;
using SFA.DAS.ApprenticeCommitments.Data.FuzzyMatching;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "GetRegistrationsReminders")]
    public class GetRegistrationsRemindersSteps
    {
        private readonly TestContext _context;
        private Fixture _fixture;
        private List<Registration> _registrations;
        private List<RegistrationTest> _testData;

        public GetRegistrationsRemindersSteps(TestContext context)
        {
            _fixture = new Fixture();
            _context = context;
            _registrations = new List<Registration>();
        }

        [Given(@"the following registration details exist")]
        public async Task GivenTheFollowingRegistrationDetailsExist(Table table)
        {
            _testData = table.CreateSet<RegistrationTest>().ToList();

            foreach (var reg in _testData)
            {
                var registration = _fixture.Create<Registration>();

                registration.SetProperty(x => x.CreatedOn, reg.CreatedOn);
                registration.SetProperty(x => x.FirstName, reg.FirstName);
                registration.SetProperty(x => x.LastName, reg.LastName);
                registration.SetProperty(x => x.Email, new MailAddress(reg.Email));
                registration.SetProperty(x => x.SignUpReminderSentOn, reg.SignUpReminderSentOn);

                _registrations.Add(registration);
                _context.DbContext.Registrations.Add(registration);
                await _context.DbContext.SaveChangesAsync();

                if(reg.ApprenticeshipConfirmed != null)
                {
                    var apprentice = _fixture.Build<Apprentice>()
                        .With(x => x.LastName, registration.LastName)
                        .With(x => x.DateOfBirth, registration.DateOfBirth)
                        .Create();
                    _context.DbContext.Add(apprentice);
                    await _context.DbContext.SaveChangesAsync();

                    var revision = _fixture.Create<Revision>();
                    revision.SetProperty(x => x.CommitmentsApprenticeshipId, registration.CommitmentsApprenticeshipId);

                    var apprenticeship = new Apprenticeship(revision);

                    apprenticeship.SetProperty(x => x.ApprenticeId, apprentice.Id);
                    registration.SetProperty(x => x.Apprenticeship, apprenticeship);

                    if (reg.ApprenticeshipConfirmed == true)
                        apprenticeship.SetProperty(x => x.ConfirmedOn, DateTime.Now);

                    _context.DbContext.Apprenticeships.Add(apprenticeship);

                    await _context.DbContext.SaveChangesAsync();
                }
            }
        }

        [When(@"we want reminders before cut off date (.*)")]
        public async Task WhenWeGetRemindersBeforeCutOffDate(DateTime cutOffTime)
        {
            await _context.Api.Get($"registrations/reminders/?invitationCutOffTime={cutOffTime:yyy-MM-dd}");
        }

        [Then(@"the result should return (.*) matching registration")]
        public async Task ThenTheResultShouldReturnMatchingRegistration(int count)
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<RegistrationRemindersResponse>(content);
            content.Should().NotBeNull();
            response.Registrations.Count.Should().Be(count);
        }

        [Then("there should be a registration with the email (.*) and it's expected values")]
        public async Task ThenThatShouldBeHasARegistrationWithTheEmailAndItSExpectedValues(string email)
        {
            var content = await _context.Api.Response.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<RegistrationRemindersResponse>(content);
            content.Should().NotBeNull();

            var expected = _testData.Find(x => x.Email == email);
            response.Registrations.Should().ContainEquivalentOf(new
            {
                expected.Email,
                expected.FirstName,
                expected.LastName,
                expected.CreatedOn,
            });
        }

        public class RegistrationTest
        {
            public string FirstName;
            public string LastName;
            public string Email;
            public DateTime? CreatedOn;
            public DateTime? SignUpReminderSentOn;
            public bool? ApprenticeshipConfirmed;
        }
    }
}