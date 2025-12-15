using System.Net;

namespace CarpetBG.Shared;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public List<string> Errors { get; } = [];
    public HttpStatusCode HttpStatusCode { get; }
    public HttpStatusCode HttpStatus { get; }

    private Result(bool isSuccess, T? value, List<string> error, HttpStatusCode httpStatusCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = error;
        HttpStatusCode = httpStatusCode;
    }

    public static Result<T> Success(T value, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        => new(true, value, [], httpStatusCode);
    public static Result<T> Failure(string error, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        => new(false, default, string.IsNullOrEmpty(error) ? [] : [error], httpStatusCode);
    public static Result<T> Failure(List<string> errors, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        => new(false, default, errors, httpStatusCode);
}
