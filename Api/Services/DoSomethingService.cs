using Hangfire;

namespace Api.Services;

[AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete, LogEvents = false)]
[DisableConcurrentExecution(15)]
public class DoSomethingService
{
    private CounterService _counterService;

    public DoSomethingService(CounterService counterService)
    {
        _counterService = counterService;
    }

    public async Task DoSomethingAsync()
    {
        var counter = await _counterService.GetCurrentAndIncrementAsync();
        Console.WriteLine($"{nameof(DoSomethingAsync)}-{counter} started: {DateTime.Now}");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Console.WriteLine($"{nameof(DoSomethingAsync)}-{counter} finished: {DateTime.Now}");
    }
}
