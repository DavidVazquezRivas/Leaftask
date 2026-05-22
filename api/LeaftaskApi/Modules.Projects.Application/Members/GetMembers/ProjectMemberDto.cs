namespace Modules.Projects.Application.Members.GetMembers;

public sealed record ProjectMemberDto(Guid Id, string Name, string? Email, Guid Role);
