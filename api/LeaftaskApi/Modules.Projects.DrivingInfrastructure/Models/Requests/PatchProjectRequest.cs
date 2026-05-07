using Modules.Projects.Domain.Entities;

namespace Modules.Projects.DrivingInfrastructure.Models.Requests;

public sealed record PatchProjectRequest
{
    public string? Name { get; init; }
    public string? Abbreviation { get; init; }
    public ProjectPrivacy? PrivacyLevel { get; init; }
}
