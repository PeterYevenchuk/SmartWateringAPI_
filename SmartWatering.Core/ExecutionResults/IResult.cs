namespace SmartWatering.Core.ExecutionResults;

public interface IResult<T>
{
    bool IsSuccess { get; }
    string ErrorMessage { get; }

    T Data { get; }
}
