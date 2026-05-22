namespace Modules.Projects.DrivingInfrastructure.Models.Requests;

public sealed record InviteProjectMemberRequest
{
    public required Guid UserId { get; init; }
    public required Guid RoleId { get; init; }
}
