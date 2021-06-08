using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.UnitTests.RenewingCommitmentStatementTests
{
    public class ApprenticeshipConfirmation
    {
        Fixture _f = new Fixture();
        private CommitmentStatement _existingCommitmentStatement;
        private Apprenticeship _apprenticeship;
        private ApprenticeshipDetails _details;
        private long _commitmentsApprenticeshipId;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApprenticeshipId = _f.Create<long>();
            _existingCommitmentStatement = _f.Create<CommitmentStatement>();
            _existingCommitmentStatement.SetProperty(p=>p.CommitmentsApprenticeshipId, _commitmentsApprenticeshipId);
            _apprenticeship = new Apprenticeship(_existingCommitmentStatement);
            _details = _existingCommitmentStatement.Details;
        }

        [TestCase(true)]
        [TestCase(false)]
        public void When_apprenticeship_section_confirmation_status_is_not_set_Then_apprenticeship_section_remains_not_set_regardless_of_data_changes(bool withSameData)
        {
            _existingCommitmentStatement.SetProperty(p => p.ApprenticeshipDetailsCorrect, null);

            var details = withSameData ? _existingCommitmentStatement.Details.Clone() : _f.Create<ApprenticeshipDetails>();

            _apprenticeship.RenewCommitment(_commitmentsApprenticeshipId, details, DateTime.Now);

            _apprenticeship.CommitmentStatements.Last().ApprenticeshipDetailsCorrect.Should().BeNull();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void When_apprenticeship_section_confirmation_status_is_set_And_no_change_to_apprenticeship_has_occurred_Then_apprenticeship_section_does_not_change_status(bool confirmationStatus)
        {
            _existingCommitmentStatement.SetProperty(p => p.ApprenticeshipDetailsCorrect, confirmationStatus);

            _apprenticeship.RenewCommitment(_commitmentsApprenticeshipId, _existingCommitmentStatement.Details.Clone(), DateTime.Now);
            
            _apprenticeship.CommitmentStatements.Last().ApprenticeshipDetailsCorrect.Should().Be(confirmationStatus);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void When_apprenticeship_section_confirmation_status_is_set_And_a_change_to_course_details_has_occurred_Then_apprenticeship_section_is_not_confirmed(bool confirmationStatus)
        {
            _existingCommitmentStatement.SetProperty(p => p.ApprenticeshipDetailsCorrect, confirmationStatus);
            var newDetails = _f.Create<ApprenticeshipDetails>();

            _apprenticeship.RenewCommitment(_commitmentsApprenticeshipId, newDetails, DateTime.Now);

            _apprenticeship.CommitmentStatements.Last().ApprenticeshipDetailsCorrect.Should().BeNull();
        }
    }
}