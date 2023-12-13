namespace Api.Services;

public class CounterService
{
    private int _count;

    public int GetNext()
    {
        return _count++;
    }
}
