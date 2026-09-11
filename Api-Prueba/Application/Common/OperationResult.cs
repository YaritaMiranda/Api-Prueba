namespace UserCurrencyApi.Application.Common;

public enum OperationStatus
{
    Success,
    NotFound,
    Conflict
}

public sealed record OperationResult<T>(OperationStatus Status, T? Value = default, string? Error = null)
{
    public static OperationResult<T> Success(T value) => new(OperationStatus.Success, value);

    public static OperationResult<T> NotFound(string error) => new(OperationStatus.NotFound, default, error);

    public static OperationResult<T> Conflict(string error) => new(OperationStatus.Conflict, default, error);
}
