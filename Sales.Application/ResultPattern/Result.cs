using System.Text;
using FluentValidation.Results;

namespace Sales.Application.ResultPattern;

public class Result<TValue> //: Result
{
    public readonly bool isSuccess;
    public readonly Error error;
    public readonly TValue? value;
    
    public readonly List<ValidationFailure> validationFailures = new List<ValidationFailure>();

    public Result(bool isSuccess, TValue value)
    {
        if (isSuccess && value == null ||
            !isSuccess && value != null)
        {
            throw new ArgumentException("Result is invalid");
        }
        this.isSuccess = isSuccess;
        this.error = Error.None;
        this.value = value;
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
    
    public Result(bool isSuccess, Error error, List<ValidationFailure> validationFailures)
    {
        if (isSuccess && error != null ||
            !isSuccess && error == null)
        {
            throw new ArgumentException("Result is invalid");
        }
        
        this.isSuccess = isSuccess;
        this.error = error;
        this.value = default;
        this.validationFailures = validationFailures;
    }
    
    public static Result<TValue> Success(TValue value) => new Result<TValue>(true, value);
    public static Result<TValue> Failure(Error error) => new Result<TValue>(false, error);
    public static Result<TValue> Failure(Error error, List<ValidationFailure> validationFailures) => new Result<TValue>(false, error, validationFailures);
    
    public string GenerateErrorResponse()
    {
        StringBuilder returnError = new StringBuilder();

        returnError.AppendLine(error.Description);

        if (validationFailures.Any())
        {
            foreach (var failures in validationFailures)
            {
                returnError.AppendLine($"{failures.PropertyName} : {{");
                returnError.AppendLine($"    {failures.ErrorMessage}");
                if(!failures.Equals(validationFailures.Last()))
                    returnError.AppendLine("},");
                else
                    returnError.AppendLine("}");
            }
        }

        return returnError.ToString();
    }
}