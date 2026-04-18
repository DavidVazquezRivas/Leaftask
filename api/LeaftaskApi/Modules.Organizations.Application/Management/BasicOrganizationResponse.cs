namespace Modules.Organizations.Application.Management;

public record BasicOrganizationResponse(
    Guid Id,
    string Name,
    string Description,
    int TotalMembers,
    int ActiveProjects,
    int CustomRoles,
    DateTime CreatedAt);
