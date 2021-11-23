﻿using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.UnitTests.RenewingRevisionTests
{
    public class RolesConfirmation
    {
        Fixture _f = new Fixture();
        private Revision _existingRevision;
        private Apprenticeship _apprenticeship;
        private long _commitmentsApprenticeshipId;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApprenticeshipId = _f.Create<long>();
            _existingRevision = _f.Create<Revision>();
            _existingRevision.SetProperty(p => p.CommitmentsApprenticeshipId, _commitmentsApprenticeshipId);
            _apprenticeship = new Apprenticeship(_existingRevision);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void When_roles_section_confirmation_status_is_not_set_Then_roles_section_remains_not_set_regardless_of_data_changes(bool withSameData)
        {
            _existingRevision.SetProperty(p => p.RolesAndResponsibilitiesConfirmations, null);

            var details = withSameData ? _existingRevision.Details.Clone() : _f.Create<ApprenticeshipDetails>();

            _apprenticeship.Revise(_commitmentsApprenticeshipId, details, DateTime.Now);

            _apprenticeship.Revisions.Last().RolesAndResponsibilitiesConfirmations.Should().BeNull();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void When_roles_section_confirmation_status_is_fully_confirmed_Then_roles_section_does_not_change_status_regardless_of_data_changes(bool withSameData)
        {
            var fullConfirmation = RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed |
                                   RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed |
                                   RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed;

            _existingRevision.SetProperty(p => p.RolesAndResponsibilitiesConfirmations, fullConfirmation);
            var details = withSameData ? _existingRevision.Details.Clone() : _f.Create<ApprenticeshipDetails>();

            _apprenticeship.Revise(_commitmentsApprenticeshipId, details, DateTime.Now);

            _apprenticeship.Revisions.Last().RolesAndResponsibilitiesConfirmations.Should().Be(fullConfirmation);
        }

        [TestCase(RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed, false)]
        [TestCase(RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed, true)]
        [TestCase(RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed, true)]
        [TestCase(RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed, false)]
        [TestCase(RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed, true)]
        [TestCase(RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed, false)]
        public void When_roles_section_confirmation_status_is_set_Then_roles_section_does_not_change_status_regardless_of_data_changes(RolesAndResponsibilitiesConfirmations? confirmationStatus, bool withSameData)
        {
            _existingRevision.SetProperty(p => p.RolesAndResponsibilitiesConfirmations, confirmationStatus);
            var details = withSameData ? _existingRevision.Details.Clone() : _f.Create<ApprenticeshipDetails>();

            _apprenticeship.Revise(_commitmentsApprenticeshipId, details, DateTime.Now);

            _apprenticeship.Revisions.Last().RolesAndResponsibilitiesConfirmations.Should().Be(confirmationStatus);
        }
    }
}