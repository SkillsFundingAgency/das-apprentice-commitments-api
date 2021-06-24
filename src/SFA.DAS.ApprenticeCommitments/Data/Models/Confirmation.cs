using System;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{

    public sealed class Confirmation
    {
        private Confirmation()
        {
        }

        private Confirmation(bool correct) =>
            (Correct, ConfirmedOn) = (correct, correct ? default : DateTime.UtcNow);

        public Confirmation Clone() =>
            new Confirmation { Correct = Correct, ConfirmedOn = ConfirmedOn };

        public bool? Correct { get; private set; }

        public DateTime? ConfirmedOn { get; private set; }

        public static implicit operator Confirmation?(bool? correct) =>
            correct == null ? default : new Confirmation(correct.Value);

        public static implicit operator bool(Confirmation? confirmation) =>
            confirmation?.Correct ?? false;
    }
}