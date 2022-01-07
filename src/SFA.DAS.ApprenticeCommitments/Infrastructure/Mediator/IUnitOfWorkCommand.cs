using MediatR;

namespace SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator
{
    public interface IUnitOfWorkCommandMarker
    {
    }

    public interface IUnitOfWorkCommand : IUnitOfWorkCommandMarker, IRequest
    {
    }

    public interface IUnitOfWorkCommand<out TResponse> : IUnitOfWorkCommandMarker, IRequest<TResponse>
    {
    }
}