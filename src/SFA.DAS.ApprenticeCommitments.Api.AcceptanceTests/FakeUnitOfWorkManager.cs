﻿using System;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.UnitOfWork.Managers;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests
{
    public class FakeUnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly Lazy<ApprenticeCommitmentsDbContext> _dbContext;

        public FakeUnitOfWorkManager(Lazy<ApprenticeCommitmentsDbContext> dbContext)
        {
            _dbContext = dbContext;
        }
        public Task BeginAsync()
        {
            return Task.CompletedTask;
        }

        public Task EndAsync(Exception ex = null)
        {
            return _dbContext.Value.SaveChangesAsync();
        }
    }
}