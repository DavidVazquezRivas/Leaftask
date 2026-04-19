namespace Modules.Organizations.Application.Management;

public record BasicOrganizationResponse(
    Guid Id,
    string Name,
    string Description,
    string Website,
    int TotalMembers,
    int ActiveProjects,
    int CustomRoles,
    DateTime CreatedAt);
