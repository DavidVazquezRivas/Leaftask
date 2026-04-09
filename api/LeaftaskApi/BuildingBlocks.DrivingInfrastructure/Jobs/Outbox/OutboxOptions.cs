namespace Modules.Users.DrivingInfrastructure.Jobs.Outbox;

public class OutboxOptions
{
    public int IntervalInSeconds { get; set; } = 5;
    public int BatchSize { get; set; } = 20;
}
