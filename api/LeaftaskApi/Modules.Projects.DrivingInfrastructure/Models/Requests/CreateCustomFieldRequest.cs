namespace Modules.Projects.DrivingInfrastructure.Models.Requests;

public sealed record CreateCustomFieldRequest
{
    public required string Name { get; init; }
    public required Guid Type { get; init; }
    public IReadOnlyList<string> Options { get; init; } = [];
    public required bool Required { get; init; }
    public IReadOnlyList<Guid> AppliesTo { get; init; } = [];
}
