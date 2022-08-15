using System;

namespace SFA.DAS.ApprenticeCommitments
{
    public static class Result
    {
        public static SuccessResult Success() => new SuccessResult();

        public static SuccessResult<T> Success<T>(T value) => new SuccessResult<T>(value);

        public static ErrorResult Error() => new ErrorResult();

        public static ErrorResult<T> Error<T>() => new ErrorResult<T>();

        public static ErrorResult<T, string> Error<T>(string message) => new ErrorResult<T, string>(message);

        public static ExceptionResult Exception(Exception exception) => new ExceptionResult(exception);

        public static ExceptionResult<T> Exception<T>(Exception exception) => new ExceptionResult<T>(exception);

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

    public interface IResult<out T> : IResult
    {
        T Data { get; }
    }

    public class SuccessResult : IResult
    {
        public bool IsSuccess => true;

        public override string ToString() => "Success";
    }

    public class SuccessResult<T> : SuccessResult, IResult<T>
    {
        public SuccessResult(T data) => Data = data;

        public T Data { get; }

        public override string ToString() => Data?.ToString() ?? $"Success<{typeof(T).Name}>";
    }

    public class ErrorResult : IResult
    {
        public bool IsSuccess => false;

        public override string ToString() => "Error";
    }

    public class ErrorResult<T> : ErrorResult, IResult<T>
    {
        public T Data => throw new Exception("Cannot access data when result is in error");
    }

    public class ErrorResult<T, E> : ErrorResult<T>, IResult<T>
    {
        public ErrorResult(E error) => Error = error;

        public E Error { get; }

        public override string ToString() => $"Error ({Error})";
    }

    public class ExceptionResult : ErrorResult
    {
        public Exception Exception { get; }

        public ExceptionResult(Exception exception) => Exception = exception;

        public override string ToString() => Exception.ToString();
    }

    public class ExceptionResult<T> : ErrorResult<T>
    {
        public Exception Exception { get; }

        public ExceptionResult(Exception exception) => Exception = exception;

        public override string ToString() => Exception.ToString();
    }

    public interface IStatusResult<out TStatus> : IResult
    {
        public TStatus Status { get; }
    }

    public class SuccessStatusResult<TStatus> : SuccessResult<TStatus>, IStatusResult<TStatus>
    {
        public TStatus Status => Data;

        public SuccessStatusResult(TStatus status) : base(status)
        {
        }
    }

    public class ExceptionStatusResult<TStatus> : ExceptionResult, IStatusResult<TStatus>
    {
        public TStatus Status { get; }

        public ExceptionStatusResult(TStatus status, Exception exception)
            : base(exception)
        {
            Status = status;
        }

        public override string ToString()
        {
            if (Status == null) return typeof(TStatus).Name;
            return $"{Status} ({base.ToString()})";
        }
    }
}