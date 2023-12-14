namespace Api.Services;

public interface ICounterService
{
    Task<int> GetCurrentAndIncrementAsync();
}
