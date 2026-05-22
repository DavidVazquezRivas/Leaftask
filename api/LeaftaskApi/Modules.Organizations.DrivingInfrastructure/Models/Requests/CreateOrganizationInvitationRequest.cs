namespace Modules.Organizations.DrivingInfrastructure.Models.Requests;

public sealed record CreateOrganizationInvitationRequest
{
    public required Guid UserId { get; init; }
    public required Guid RoleId { get; init; }
}
