using System;

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public interface IStoppable
    {
        void Stop(DateTime now);
    }
}