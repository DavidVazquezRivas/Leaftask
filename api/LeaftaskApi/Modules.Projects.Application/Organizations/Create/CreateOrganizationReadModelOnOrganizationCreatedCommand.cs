using BuildingBlocks.Application.Commands;

namespace Modules.Projects.Application.Organizations.Create;

public sealed record CreateOrganizationReadModelOnOrganizationCreatedCommand(Guid OrganizationId) : ICommand;
