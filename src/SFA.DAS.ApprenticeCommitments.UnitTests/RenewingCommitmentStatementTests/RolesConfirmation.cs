using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.UnitTests.RenewingCommitmentStatementTests
{
    public class RolesConfirmation
    {
        Fixture _f = new Fixture();
        private CommitmentStatement _existingCommitmentStatement;
        private Apprenticeship _apprenticeship;
        private long _commitmentsApprenticeshipId;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApprenticeshipId = _f.Create<long>();
            _existingCommitmentStatement = _f.Create<CommitmentStatement>();
            _existingCommitmentStatement.SetProperty(p => p.CommitmentsApprenticeshipId, _commitmentsApprenticeshipId);
            _apprenticeship = new Apprenticeship(_existingCommitmentStatement);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void When_roles_section_confirmation_status_is_not_set_Then_roles_section_remains_not_set_regardless_of_data_changes(bool withSameData)
        {
            _existingCommitmentStatement.SetProperty(p => p.RolesAndResponsibilitiesCorrect, null);

            var details = withSameData ? _existingCommitmentStatement.Details.Clone() : _f.Create<ApprenticeshipDetails>();

            _apprenticeship.RenewCommitment(_commitmentsApprenticeshipId, details, DateTime.Now);

            _apprenticeship.CommitmentStatements.Last().RolesAndResponsibilitiesCorrect.Should().BeNull();
        }

        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void When_roles_section_confirmation_status_is_set_Then_roles_section_does_not_change_status_regardless_of_data_changes(bool confirmationStatus, bool withSameData)
        {
            _existingCommitmentStatement.SetProperty(p => p.RolesAndResponsibilitiesCorrect, confirmationStatus);
            var details = withSameData ? _existingCommitmentStatement.Details.Clone() : _f.Create<ApprenticeshipDetails>();

            _apprenticeship.RenewCommitment(_commitmentsApprenticeshipId, details, DateTime.Now);
            
            _apprenticeship.CommitmentStatements.Last().RolesAndResponsibilitiesCorrect.Should().Be(confirmationStatus);
        }
    }
}