namespace SmartWatering.Core.ExecutionResults;

public class ExecutionResult<T> : IExecutionResult<T>
{
    public Task<IResult<T>> Fail(string error)
    {
        return Task.FromResult<IResult<T>>(new FailureResult<T>(error));
    }

    public Task<IResult<T>> Successful(T result)
    {
        return Task.FromResult<IResult<T>>(new SuccessResult<T>(result));
    }
}
