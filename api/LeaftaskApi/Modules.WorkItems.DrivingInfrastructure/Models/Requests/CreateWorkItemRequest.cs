namespace Modules.WorkItems.DrivingInfrastructure.Models.Requests;

public sealed class CreateWorkItemRequest
{
    public required string Title { get; init; }
    public required Guid ParentId { get; init; }
    public required Guid TypeId { get; init; }
    public required Guid StatusId { get; init; }
    public required decimal Estimation { get; init; }
    public string Description { get; init; } = string.Empty;
    public Guid? AssigneeId { get; init; }
    public IReadOnlyDictionary<Guid, string> CustomFields { get; init; } =
        new Dictionary<Guid, string>();
}
