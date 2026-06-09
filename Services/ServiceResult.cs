namespace EventRegistrationsApi.Services;


public class ServiceResult<T>
{
    public bool IsSuccess { get; private init; }
    public T? Value { get; private init; }
    public string? Error { get; private init; }
    public int StatusCode { get; private init; }

    public static ServiceResult<T> Ok(T value) =>
        new() { IsSuccess = true, Value = value, StatusCode = 200 };

    public static ServiceResult<T> Created(T value) =>
        new() { IsSuccess = true, Value = value, StatusCode = 201 };

    public static ServiceResult<T> Fail(string error, int statusCode = 400) =>
        new() { IsSuccess = false, Error = error, StatusCode = statusCode };
}
