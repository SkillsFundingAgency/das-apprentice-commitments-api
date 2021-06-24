using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.UnitTests.RenewingCommitmentStatementTests
{
    public class EmployerConfirmation
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
        public void When_employer_section_confirmation_status_is_not_set_Then_employer_section_remains_not_set_regardless_of_data_changes(bool withSameData)
        {
            var details = withSameData ? _existingCommitmentStatement.Details.Clone() : _f.Create<ApprenticeshipDetails>();

            _apprenticeship.RenewCommitment(_commitmentsApprenticeshipId, details, DateTime.Now);

            _apprenticeship.CommitmentStatements.Last().EmployerCorrect.Should().BeNull();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void When_employer_section_confirmation_status_is_set_And_no_change_to_employer_has_occurred_Then_employer_section_does_not_change_status(bool confirmationStatus)
        {
            _existingCommitmentStatement.Confirm(new Confirmations { EmployerCorrect = confirmationStatus }, DateTime.UtcNow);

            _apprenticeship.RenewCommitment(_commitmentsApprenticeshipId, _existingCommitmentStatement.Details.Clone(), DateTime.Now);

            _apprenticeship.CommitmentStatements.Last().EmployerCorrect.Should().Be(confirmationStatus);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void When_employer_section_confirmation_status_is_set_And_a_change_to_employer_name_has_occurred_Then_employer_section_is_not_confirmed(bool confirmationStatus)
        {
            _existingCommitmentStatement.Confirm(new Confirmations { EmployerCorrect = confirmationStatus }, DateTime.UtcNow);
            var newDetails = _f.Create<ApprenticeshipDetails>();
            newDetails.SetProperty(p=>p.EmployerAccountLegalEntityId, _existingCommitmentStatement.Details.EmployerAccountLegalEntityId);

            _apprenticeship.RenewCommitment(_commitmentsApprenticeshipId, newDetails, DateTime.Now);

            _apprenticeship.CommitmentStatements.Last().EmployerCorrect.Should().BeNull();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void When_employer_section_confirmation_is_set_And_a_change_to_employer_id_has_occurred_Then_employer_section_is_not_confirmed(bool confirmationStatus)
        {
            _existingCommitmentStatement.Confirm(new Confirmations { EmployerCorrect = confirmationStatus }, DateTime.UtcNow);
            var newDetails = _f.Create<ApprenticeshipDetails>();
            newDetails.SetProperty(p => p.EmployerName, _existingCommitmentStatement.Details.EmployerName);

            _apprenticeship.RenewCommitment(_commitmentsApprenticeshipId, newDetails, DateTime.Now);
            _apprenticeship.CommitmentStatements.Last().EmployerCorrect.Should().BeNull();
        }
    }
}