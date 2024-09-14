using System.Net;

namespace Sales.Application.ResultPattern;

public class Result<TValue>
{
    public readonly bool isSuccess;
    public readonly TValue? value;
    public readonly Error error;

    public Result(bool isSuccess, TValue value)
    {
        if (isSuccess && value == null ||
            !isSuccess && value != null)
        {
            throw new Exception("Result is invalid");
        }
        
        this.isSuccess = isSuccess;
        this.value = value;
        this.error = Error.None;
    }
    
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != null ||
            !isSuccess && error == null)
        {
            throw new ArgumentException("Result is invalid");
        }
        
        this.isSuccess = isSuccess;
        this.error = error;
        this.value = default;
    }
    
    public static Result<TValue> Success(TValue value) => new Result<TValue>(true, value);
    public static Result<TValue> Failure(Error error) => new Result<TValue>(false, error);
}