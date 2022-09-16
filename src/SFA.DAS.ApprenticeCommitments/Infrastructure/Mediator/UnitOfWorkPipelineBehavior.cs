﻿using MediatR;
using SFA.DAS.UnitOfWork.Managers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator
{
    public class UnitOfWorkPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UnitOfWorkPipelineBehavior(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!ShouldHandleRequest(request))
            {
                return await next();
            }

            await _unitOfWorkManager.BeginAsync();

            try
            {
                var response = await next();
                await _unitOfWorkManager.EndAsync();
                return response;
            }
            catch (Exception e)
            {
                await _unitOfWorkManager.EndAsync(e);
                throw;
            }
        }

        private bool ShouldHandleRequest(TRequest request) => request is IUnitOfWorkCommandMarker;
    }
}