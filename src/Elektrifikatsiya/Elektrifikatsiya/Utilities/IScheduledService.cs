namespace Elektrifikatsiya.Utilities;

public interface IScheduledService
{
    DateTime FirstExecutionTime { get; }
    TimeSpan ExecutionRepeatDelay { get; }

    void Update();
}