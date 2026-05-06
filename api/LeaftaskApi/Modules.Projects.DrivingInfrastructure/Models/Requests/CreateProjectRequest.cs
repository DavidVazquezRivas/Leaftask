namespace Modules.Projects.DrivingInfrastructure.Models.Requests;

public sealed record CreateProjectRequest
{
    public required string Name { get; init; }
    public required string Abbreviation { get; init; }
    public required Guid PrivacyLevelId { get; init; }
    public Guid? OrganizationId { get; init; }
}
