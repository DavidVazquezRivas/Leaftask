namespace Modules.WorkItems.DrivingInfrastructure.Models.Requests;

public sealed class UpdateWorkLogRequest
{
    public decimal? Dedication { get; init; }
    public DateOnly? Date { get; init; }
    public string? Description { get; init; }
}
