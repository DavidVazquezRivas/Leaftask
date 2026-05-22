namespace Modules.Projects.DrivingInfrastructure.Models.Requests;

public sealed record UpdateProjectMemberRoleRequest
{
    public required Guid RoleId { get; init; }
}
