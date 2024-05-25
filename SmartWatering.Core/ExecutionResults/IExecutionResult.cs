namespace SmartWatering.Core.ExecutionResults;

public interface IExecutionResult<T>
{
    Task<IResult<T>> Successful(T result);
    Task<IResult<T>> Fail(string error);
}
