using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;
using System.Collections.Generic;

#nullable disable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.CreateApprenticeshipFromRegistrationCommand
{
    public class CreateApprenticeshipFromRegistrationCommand : IUnitOfWorkCommand<Result>
    {
        public Guid RegistrationId { get; set; }
        public Guid ApprenticeId { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}

namespace SFA.DAS.ApprenticeCommitments
{
    public abstract class Result
    {
        public bool Success { get; protected set; }
        public bool Failure => !Success;
    }

    public abstract class Result<T> : Result
    {
        private T _data;

        protected Result(T data)
        {
            Data = data;
        }

        public T Data
        {
            get => Success ? _data : throw new Exception($"You can't access .{nameof(Data)} when .{nameof(Success)} is false");
            set => _data = value;
        }
    }

    public class SuccessResult : Result
    {
        public SuccessResult()
        {
            Success = true;
        }
    }

    public class SuccessResult<T> : Result<T>
    {
        public SuccessResult(T data) : base(data)
        {
            Success = true;
        }
    }

    public class ExceptionResult : Result, IErrorResult
    {
        public ExceptionResult(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }

        public string Message => Exception.Message;

        public IReadOnlyCollection<Error> Errors { get; } = Array.Empty<Error>();
    }

    public class ErrorResult : Result, IErrorResult
    {
        public ErrorResult(string message) : this(message, Array.Empty<Error>())
        {
        }

        public ErrorResult(string message, IReadOnlyCollection<Error> errors)
        {
            Message = message;
            Success = false;
            Errors = errors ?? Array.Empty<Error>();
        }

        public string Message { get; }
        public IReadOnlyCollection<Error> Errors { get; }
    }

    public class ErrorResult<T> : Result<T>, IErrorResult
    {
        public ErrorResult(string message) : this(message, Array.Empty<Error>())
        {
        }

        public ErrorResult(string message, IReadOnlyCollection<Error> errors) : base(default)
        {
            Message = message;
            Success = false;
            Errors = errors ?? Array.Empty<Error>();
        }

        public string Message { get; set; }
        public IReadOnlyCollection<Error> Errors { get; }
    }

    public class Error
    {
        public Error(string details) : this(null, details)
        {
        }

        public Error(string code, string details)
        {
            Code = code;
            Details = details;
        }

        public string Code { get; }
        public string Details { get; }
    }

    internal interface IErrorResult
    {
        string Message { get; }
        IReadOnlyCollection<Error> Errors { get; }
    }
}