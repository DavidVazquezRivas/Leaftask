using Modules.Projects.Domain.Entities;

namespace Modules.Projects.Application.Management.Create;

public sealed record ProjectResponse(
    Guid Id,
    string Name,
    string Abbreviation,
    ProjectPrivacy PrivacyLevel,
    Guid? OrganizationId,
    DateTime CreatedAt);
