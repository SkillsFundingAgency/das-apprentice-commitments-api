using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmEmployerCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ApprenticeCommitments.Exceptions;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public class Apprenticeship
    {
        private Apprenticeship()
        {
        }

        public Apprenticeship(CommitmentStatement apprenticeship)
        {
            CommitmentStatements.Add(apprenticeship);
        }

        public long Id { get; private set; }

        public Apprentice Apprentice { get; private set; } = null!;

        public ICollection<CommitmentStatement> CommitmentStatements { get; private set; }
            = new List<CommitmentStatement>();

        public CommitmentStatement LatestCommitmentStatement
            => CommitmentStatements.OrderByDescending(x => x.CommitmentsApprovedOn).FirstOrDefault();

        internal void ConfirmApprenticeship(bool apprenticeshipCorrect)
            => LatestCommitmentStatement.ConfirmApprenticeship(apprenticeshipCorrect);

        internal void ConfirmApprenticeshipDetails(bool apprenticeshipCorrect)
            => LatestCommitmentStatement.ConfirmApprenticeshipDetails(apprenticeshipCorrect);

        internal void ConfirmEmployer(ConfirmEmployerData confirmEmployerCorrect)
        {
            var cs = LatestCommitmentStatement;
                
            if(cs.Id != confirmEmployerCorrect.ComitmentStatementId)
                throw new DomainException("Commitment Statement has been updated");

            cs.ConfirmEmployer(confirmEmployerCorrect.EmployerCorrect);
        }

        internal void ConfirmTrainingProvider(bool apprenticeshipCorrect)
            => LatestCommitmentStatement.ConfirmTrainingProvider(apprenticeshipCorrect);

        internal void ConfirmRolesAndResponsibilities(bool apprenticeshipCorrect)
            => LatestCommitmentStatement.ConfirmRolesAndResponsibilities(apprenticeshipCorrect);

        internal void ConfirmHowApprenticeshipWillBeDelivered(bool apprenticeshipCorrect)
            => LatestCommitmentStatement.ConfirmHowApprenticeshipWillBeDelivered(apprenticeshipCorrect);

        internal void RenewCommitment(ApprenticeshipDetails details, DateTime approvedOn)
        {
            var ns = CommitmentStatements.OrderByDescending(x => x.CommitmentsApprovedOn).First().RenewCommitment(details, approvedOn);
            CommitmentStatements.Add(ns);
        }
    }
}