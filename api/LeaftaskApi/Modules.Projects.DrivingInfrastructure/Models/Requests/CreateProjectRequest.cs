using Modules.Projects.Domain.Entities;

namespace Modules.Projects.DrivingInfrastructure.Models.Requests;

public sealed record CreateProjectRequest
{
    public required string Name { get; init; }
    public required string Abbreviation { get; init; }
    public required ProjectPrivacy PrivacyLevel { get; init; }
    public Guid? OrganizationId { get; init; }
}
