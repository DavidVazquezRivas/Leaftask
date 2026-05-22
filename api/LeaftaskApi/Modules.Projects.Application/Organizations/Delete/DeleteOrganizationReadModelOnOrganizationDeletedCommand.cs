using BuildingBlocks.Application.Commands;

namespace Modules.Projects.Application.Organizations.Delete;

public sealed record DeleteOrganizationReadModelOnOrganizationDeletedCommand(Guid OrganizationId) : ICommand;
