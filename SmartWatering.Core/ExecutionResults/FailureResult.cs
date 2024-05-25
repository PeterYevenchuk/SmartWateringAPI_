namespace SmartWatering.Core.ExecutionResults;

public class FailureResult<T> : IResult<T>
{
    public bool IsSuccess => false;
    public string ErrorMessage { get; }

    public T Data => throw new InvalidOperationException("There is no data for failure result.");

    public FailureResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}