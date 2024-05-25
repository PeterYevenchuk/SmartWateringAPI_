namespace SmartWatering.Core.ExecutionResults;

public class SuccessResult<T> : IResult<T>
{
    public bool IsSuccess => true;
    public string ErrorMessage => string.Empty;

    public T Data { get; }

    public SuccessResult(T data)
    {
        Data = data;
    }
}
