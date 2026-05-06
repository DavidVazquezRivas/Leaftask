namespace Modules.Projects.Application.Management.Create;

public sealed record ProjectResponse(
    Guid Id,
    string Name,
    string Abbreviation,
    PrivacyLevelDto PrivacyLevel,
    Guid? OrganizationId,
    DateTime CreatedAt);
