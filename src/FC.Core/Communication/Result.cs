using System.Diagnostics.CodeAnalysis;

namespace FC.Core.Communication;

/// <summary>
///     Represents the result of an operation, including whether it succeeded and an associated list of errors if it
///     failed.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, List<Error>? errors)
    {
        switch (isSuccess)
        {
            case true when errors?.Count > 0:
                throw new InvalidOperationException("A successful result cannot have errors.");
            case false when errors?.Count == 0:
                throw new InvalidOperationException("A failed result must have at least one error.");
            default:
                IsSuccess = isSuccess;
                Errors = errors ?? [];
                break;
        }
    }

    /// <summary>
    ///     Indicates whether the result is a success.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    ///     Indicates whether the result is a failure.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    ///     The list of errors associated with a failed result.
    /// </summary>
    public List<Error> Errors { get; }

    /// <summary>
    ///     Creates a successful result.
    /// </summary>
    public static Result Success()
    {
        return new Result(true, null);
    }

    /// <summary>
    ///     Creates a failed result with the specified list of errors.
    /// </summary>
    public static Result Failure(List<Error> errors)
    {
        return new Result(false, errors.ToList());
    }

    /// <summary>
    ///     Creates a successful result with a value.
    /// </summary>
    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value, true, null);
    }

    /// <summary>
    ///     Creates a failed result with the specified list of errors.
    /// </summary>
    public static Result<T> Failure<T>(List<Error> errors)
    {
        return new Result<T>(default, false, errors.ToList());
    }

    /// <summary>
    ///     Creates a result based on the value. If the value is null, it returns a failed result.
    /// </summary>
    public static Result<T> Create<T>(T? value)
    {
        return value is not null ? Success(value) : Failure<T>([Error.NullValue]);
    }
}

/// <summary>
///     Represents the result of an operation that returns a value.
/// </summary>
public class Result<T> : Result
{
    private readonly T? _value;

    internal Result(T? value, bool isSuccess, List<Error>? errors)
        : base(isSuccess, errors)
    {
        _value = value;
    }

    /// <summary>
    ///     Gets the value of the result. Throws InvalidOperationException if the result is a failure.
    /// </summary>
    [NotNull]
    public T Value => _value ?? throw new InvalidOperationException("Result has no value");

    public static implicit operator Result<T>(T? value)
    {
        return Create(value);
    }
}

// Extending the Result pattern with helper methods 
public static class ResultWithErrorListExtensions
{
    /// <summary>
    ///     Applies the specified action if the result is a success.
    /// </summary>
    public static void OnSuccess(this Result result, Action action)
    {
        if (result.IsSuccess) action();
    }

    /// <summary>
    ///     Applies the specified action if the result is a failure.
    /// </summary>
    public static void OnFailure(this Result result, Action<List<Error>> action)
    {
        if (result.IsFailure) action(result.Errors);
    }

    /// <summary>
    ///     Applies the specified action with the value if the result is a success.
    /// </summary>
    public static void OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess) action(result.Value);
    }

    /// <summary>
    ///     Applies the specified action if the result is a failure.
    /// </summary>
    public static void OnFailure<T>(this Result<T> result, Action<List<Error>> action)
    {
        if (result.IsFailure) action(result.Errors);
    }
}