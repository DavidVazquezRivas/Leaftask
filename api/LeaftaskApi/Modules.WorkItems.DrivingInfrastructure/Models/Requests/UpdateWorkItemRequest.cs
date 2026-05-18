namespace Modules.WorkItems.DrivingInfrastructure.Models.Requests;

public sealed class UpdateWorkItemRequest
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public Guid? StatusId { get; init; }
    public Guid? TypeId { get; init; }
    public Guid? AssigneeId { get; init; }
    public bool? UpdateAssignee { get; init; }
    public int? Progress { get; init; }
    public decimal? Estimation { get; init; }
    public DateTime? LimitDate { get; init; }
    public Guid? ParentId { get; init; }
    public bool? UpdateParent { get; init; }
    public IReadOnlyDictionary<Guid, string> CustomFields { get; init; } =
        new Dictionary<Guid, string>();
}
