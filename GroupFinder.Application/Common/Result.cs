﻿namespace GroupFinder.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    public T Value { get; }

    private Result(bool isSuccess, string error, T value)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }
    
    public static Result<T> Success(T value) => new(true, string.Empty, value);
    public static Result<T> Failure(string error) => new(false, error, default!);
}