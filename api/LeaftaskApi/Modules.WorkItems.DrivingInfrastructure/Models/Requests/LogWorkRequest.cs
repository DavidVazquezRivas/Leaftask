namespace Modules.WorkItems.DrivingInfrastructure.Models.Requests;

public sealed class LogWorkRequest
{
    public required decimal Dedication { get; init; }
    public required DateOnly Date { get; init; }
    public required string Description { get; init; }
}
