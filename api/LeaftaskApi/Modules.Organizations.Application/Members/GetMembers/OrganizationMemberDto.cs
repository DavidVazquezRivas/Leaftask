namespace Modules.Organizations.Application.Members.GetMembers;

public sealed record OrganizationMemberDto(
    Guid Id,
    string Name,
    string Email,
    Guid Role);
