namespace DashboardData.Services;

public class UserCounterService
{
    public int Count { get; private set; } = 0;

    public void Increment()
    {
        Count++;
    }
}
