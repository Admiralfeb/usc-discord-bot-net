namespace UnitedSystemsCooperative.Bot.Utils;

public static class UtilityMethods
{
    public static async Task PeriodicAsync(Func<Task> action, TimeSpan interval, CancellationToken cancellationToken = default)
    {
        using var timer = new PeriodicTimer(interval);
        while (true)
        {
            await action();
            await timer.WaitForNextTickAsync(cancellationToken);
        }
    }
}
