namespace BTB.Data;

/// <summary>
/// Represents the possible outcomes of an operation.
/// </summary>
public enum ResultStatus
{
    /// <summary>
    /// The operation completed successfully.
    /// </summary>
    Success,

    /// <summary>
    /// The requested resource was not found.
    /// </summary>
    NotFound,

    /// <summary>
    /// The operation failed validation checks.
    /// </summary>
    ValidationError,

    /// <summary>
    /// The operation could not be completed due to a conflict.
    /// </summary>
    Conflict,

    /// <summary>
    /// The operation failed unexpectedly.
    /// </summary>
    Failure,

    /// <summary>
    /// The operation was canceled.
    /// </summary>
    Canceled
}

/// <summary>
/// Represents the result of an operation, including its status and any associated error message.
/// </summary>
public class Result
{
    #region Public Properties
    public ResultStatus Status { get; }
    public string? Message { get; }
    public string? ErrorCode { get; }
    public bool IsSuccess => Status == ResultStatus.Success;
    public bool IsFailure => Status == ResultStatus.Failure;
    public bool IsNotFound => Status == ResultStatus.NotFound;
    public bool IsValidationError => Status == ResultStatus.ValidationError;
    public bool IsConflict => Status == ResultStatus.Conflict;
    public bool IsCanceled => Status == ResultStatus.Canceled;
    #endregion

    // Constructor
    public Result(ResultStatus status, string? message = null, string? errorCode = null)
    {
        Status = status;
        Message = message;
        ErrorCode = errorCode;
    }

    #region Public Methodes
    public static Result Success(string? message = null) => new(ResultStatus.Success, message);
    public static Result NotFound(string? message = null) => new(ResultStatus.NotFound, message);
    public static Result ValidationError(string? message = null) => new(ResultStatus.ValidationError, message);
    public static Result Conflict(string? message = null) => new(ResultStatus.Conflict, message);
    public static Result Failure(string? message = null) => new(ResultStatus.Failure, message);
    public static Result Canceled(string? message = null) => new(ResultStatus.Canceled, message);
    #endregion
}

/// <summary>
/// Represents the result of an operation that returns a value of type T, including its status and any associated error message.
/// </summary>
public class Result<T> : Result
{
    #region Public Properties
    public T? Value { get; }
    #endregion 

    // Constructor
    private Result(ResultStatus status, T? value = default, string? message = null, string? errorCode = null) : base(status, message, errorCode)
    {
        Value = value;
    }

    #region Public Methods
    public static Result<T> Success(T value, string? message = null) => new(ResultStatus.Success, value, message);
    public static new Result<T> NotFound(string? message = null) => new(ResultStatus.NotFound, default, message);
    public static new Result<T> ValidationError(string? message = null) => new(ResultStatus.ValidationError, default, message);
    public static new Result<T> Conflict(string? message = null) => new(ResultStatus.Conflict, default, message);
    public static new Result<T> Failure(string? message = null) => new(ResultStatus.Failure, default, message);
    public static new Result<T> Canceled(string? message = null) => new(ResultStatus.Canceled, default, message);

    /// <summary>
    /// Attempts to get the result value.
    /// Returns true only when the result was successful and contains a value.
    /// </summary>
    public bool TryGetValue(out T? value)
    {
        if (!IsSuccess)
        {
            value = default!;
            return false;
        }

        value = Value;
        return true;
    }
    #endregion
}
