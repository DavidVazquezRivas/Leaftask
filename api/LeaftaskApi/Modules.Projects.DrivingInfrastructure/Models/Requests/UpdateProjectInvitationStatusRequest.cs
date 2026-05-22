namespace Modules.Projects.DrivingInfrastructure.Models.Requests;

public sealed record UpdateProjectInvitationStatusRequest
{
    public required string Status { get; init; }
}
