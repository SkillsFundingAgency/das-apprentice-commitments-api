﻿using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.WorkflowTests
{
    internal class ChangeOfPersonalDetailsFixture : ApiFixture
    {
        public async Task<(CreateRegistrationCommand approval, ChangeRegistrationCommand create)> CreateRegistrationAndCoc()
        {
            var approval = await CreateRegistration();
            ChangeRegistrationCommand coc = CreateChangeOfPersonalDetails(approval);

            Reset();

            return (approval, coc);
        }

        protected ChangeRegistrationCommand CreateChangeOfPersonalDetails(CreateRegistrationCommand approval)
        {
            var coc = fixture.Create<ChangeRegistrationCommand>();
            coc.CommitmentsApprenticeshipId = approval.CommitmentsApprenticeshipId;
            coc.CommitmentsContinuedApprenticeshipId = null;
            coc.CommitmentsApprovedOn = approval.CommitmentsApprovedOn;
            coc.CourseDuration = approval.CourseDuration;
            coc.CourseLevel = approval.CourseLevel;
            coc.CourseName = approval.CourseName;
            coc.CourseOption = approval.CourseOption;
            coc.EmployerAccountLegalEntityId = approval.EmployerAccountLegalEntityId;
            coc.EmployerName = approval.EmployerName;
            coc.PlannedEndDate = approval.PlannedEndDate;
            coc.PlannedStartDate = approval.PlannedStartDate;
            coc.TrainingProviderId = approval.TrainingProviderId;
            coc.TrainingProviderName = approval.TrainingProviderName;
            return coc;
        }
    }

    internal class When_change_of_personal_details_before_apprenticeship_is_matched : ChangeOfPersonalDetailsFixture
    {
        [Test]
        public async Task Updates_personal_details_in_registration()
        {
            var (approval, coc) = await CreateRegistrationAndCoc();

            await ChangeOfCircumstances(coc);

            var registration = Database.Registrations.Find(approval.RegistrationId);
            registration.Should().BeEquivalentTo(new
            {
                approval.RegistrationId,
                coc.FirstName,
                coc.LastName,
                coc.DateOfBirth,
                Email = new MailAddress(coc.Email),
            });
        }

        [Test]
        public async Task Triggers_ApprenticeshipRegisteredEvent()
        {
            var (approval, coc) = await CreateRegistrationAndCoc();

            await ChangeOfCircumstances(coc);

            PublishedNServiceBusEvents.Should().ContainEquivalentOf(new
            {
                Event = new ApprenticeshipRegisteredEvent
                {
                    RegistrationId = approval.RegistrationId,
                }
            });
        }
    }

    internal class When_change_of_personal_details_includes_email_belonging_to_matched_account : ChangeOfPersonalDetailsFixture
    {
        [Test, AutoData]
        public async Task Updates_personal_details_in_registration(MailAddress existingEmail)
        {
            await CreateVerifiedApprenticeship(email: existingEmail);
            var (approval, coc) = await CreateRegistrationAndCoc();
            coc.Email = existingEmail.ToString();

            await ChangeOfCircumstances(coc);

            var registration = Database.Registrations.Find(approval.RegistrationId);
            registration.Should().BeEquivalentTo(new
            {
                approval.RegistrationId,
                coc.FirstName,
                coc.LastName,
                coc.DateOfBirth,
                Email = new MailAddress(coc.Email),
            });
        }

        [Test, AutoData]
        public async Task Triggers_ApprenticeshipRegisteredEvent(MailAddress existingEmail)
        {
            await CreateVerifiedApprenticeship(email: existingEmail);
            var (approval, coc) = await CreateRegistrationAndCoc();
            coc.Email = existingEmail.ToString();

            await ChangeOfCircumstances(coc);

            PublishedNServiceBusEvents.Should().ContainEquivalentOf(new
            {
                Event = new ApprenticeshipRegisteredEvent
                {
                    RegistrationId = approval.RegistrationId,
                }
            });
        }
    }
}