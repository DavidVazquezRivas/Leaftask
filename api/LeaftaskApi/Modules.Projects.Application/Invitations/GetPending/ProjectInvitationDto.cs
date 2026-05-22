namespace Modules.Projects.Application.Invitations.GetPending;

public sealed record ProjectInvitationDto(
    Guid Id,
    ProjectInvitationUserDto User,
    ProjectInvitationRoleDto Role);

public sealed record ProjectInvitationUserDto(Guid Id, string Name, string? Email);

public sealed record ProjectInvitationRoleDto(Guid Id, string Name);
