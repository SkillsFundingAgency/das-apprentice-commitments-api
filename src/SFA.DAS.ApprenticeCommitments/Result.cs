using System;

namespace SFA.DAS.ApprenticeCommitments
{
    public partial class ResultX
    {
        public static SuccessStatusResult<TStatus> SuccessStatus<TStatus>(TStatus status)
            => new SuccessStatusResult<TStatus>(status);

        public static ExceptionStatusResult<TStatus> ExceptionStatus<TStatus>(TStatus status, Exception exception)
            => new ExceptionStatusResult<TStatus>(status, exception);
    }

    public interface IResult
    {
        public bool IsSuccess { get; }
        public bool IsError => !IsSuccess;
    }

    public interface IResult<T> : IResult
    {
        T Data { get; }
    }

    public class ExceptionResult2 : IResult
    {
        public bool IsSuccess => false;

        public Exception Exception { get; }

        public ExceptionResult2(Exception exception) => Exception = exception;
    }

    public interface IStatusResult<TStatus> : IResult
    {
        public TStatus Status { get; }
    }

    public class SuccessStatusResult<TStatus> : SuccessResult2, IStatusResult<TStatus>
    {
        public TStatus Status { get; }

        public SuccessStatusResult(TStatus status) => Status = status;
    }

    public class ExceptionStatusResult<TStatus> : ExceptionResult2, IStatusResult<TStatus>
    {
        public TStatus Status { get; }

        public ExceptionStatusResult(TStatus status, Exception exception)
            : base(exception)
        {
            Status = status;
        }
    }

    public class SuccessResult2 : IResult
    {
        public bool IsSuccess => true;
    }

    public class SuccessResult2<T> : SuccessResult2, IResult<T>
    {
        public SuccessResult2(T data) => Data = data;

        public T Data { get; }
    }

    public class ErrorResult2 : IResult
    {
        public bool IsSuccess => false;
    }

    public class ErrorResult2<T, E> : ErrorResult2, IResult<T>
    {
        public ErrorResult2(E error) => Error = error;

        public E Error { get; }

        public T Data => throw new Exception("Cannot access data when result is in error");
    }

    public abstract class Result : IResult
    {
        public bool IsSuccess { get; protected set; }
        public bool IsFailure => !IsSuccess;
    }

    public abstract class Result<T> : Result
    {
        private readonly T _data;

        protected Result(T data) => _data = data;

        public T Data => IsSuccess
            ? _data
            : throw new Exception($"You can't access .{nameof(Data)} when .{nameof(IsSuccess)} is false");
    }

    public class SuccessResult : Result
    {
        public SuccessResult() => IsSuccess = true;
    }

    public class SuccessResult<T> : Result<T>
    {
        public SuccessResult(T data) : base(data) => IsSuccess = true;
    }

    public class ExceptionResult : Result, IErrorResult
    {
        public ExceptionResult(Exception exception)
            => Exception = exception ?? throw new ArgumentNullException(nameof(exception));

        public Exception Exception { get; }

        public string Message => Exception.Message;
    }

    internal interface IErrorResult
    {
        string Message { get; }
    }
}