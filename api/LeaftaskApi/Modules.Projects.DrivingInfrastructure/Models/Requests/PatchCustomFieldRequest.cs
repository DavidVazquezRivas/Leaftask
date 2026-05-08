namespace Modules.Projects.DrivingInfrastructure.Models.Requests;

public sealed record PatchCustomFieldRequest
{
    public string? Name { get; init; }
    public Guid? Type { get; init; }
    public IReadOnlyList<string>? Options { get; init; }
    public bool? Required { get; init; }
    public IReadOnlyList<Guid>? AppliesTo { get; init; }
}
