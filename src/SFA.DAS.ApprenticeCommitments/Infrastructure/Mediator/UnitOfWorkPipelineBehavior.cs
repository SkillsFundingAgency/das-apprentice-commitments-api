using MediatR;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator
{
    public class UnitOfWorkPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ApprenticeCommitmentsDbContext _dbContext;

        public UnitOfWorkPipelineBehavior(ApprenticeCommitmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!ShouldHandleRequest(request))
            {
                return await next();
            }

            var transaction = await _dbContext.Database.BeginTransactionAsync(CancellationToken.None);

            try
            {
                var response = await next();
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(CancellationToken.None);
                return response;
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }

        private static bool ShouldHandleRequest(TRequest request) => request is IUnitOfWorkCommandMarker;

    }
}